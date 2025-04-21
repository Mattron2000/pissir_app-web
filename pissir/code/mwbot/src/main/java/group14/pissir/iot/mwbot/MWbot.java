package group14.pissir.iot.mwbot;

import java.util.ArrayList;
import java.util.Random;
import java.util.Scanner;

import group14.pissir.util.IotDeviceType;
import group14.pissir.util.JsonWrapper;
import group14.pissir.util.MWbotStatus;
import group14.pissir.util.ModelCar;
import group14.pissir.util.MqttClientWrapper;
import group14.pissir.util.MqttUtil;
import group14.pissir.util.TOPICS;
import group14.pissir.util.schema.MWbotRequest;
import group14.pissir.util.schema.MWbotResponse;
import group14.pissir.util.schema.MobileNotification;
import group14.pissir.util.schema.MwBotDataDebug;

public class MWbot {

    private static final int CHARGE_KW_PER_SEC = 1;

    MqttClientWrapper mqttClient;

    private ArrayList<MWbotRequest> queue;
    private MWbotStatus status;
    private Integer position;

    private MWbot(Boolean debugLog) {
        this.position = 0;
        this.status = MWbotStatus.IDLE;
        this.queue = new ArrayList<>();

        ArrayList<String> subscribeTopics = new ArrayList<>();
        subscribeTopics.add(TOPICS.MWBOT.getNewRequest());
        subscribeTopics.add(TOPICS.getLwt());

        this.mqttClient = new MqttClientWrapper(MqttUtil.MWbot.id, IotDeviceType.MWBOT, debugLog, subscribeTopics, new MWbotCallBack(this));
    }

    void run() {
        ModelCar car = null;
        MWbotRequest request = null;

        this.setStatus(MWbotStatus.IDLE);
        this.publishDebug(this.status, this.position, null, null);
        while (!this.queueIsEmpty()) {
            request = this.getNewRequest();
            if (car == null)
                this.publishDebug(this.status, this.position, null, request.percentage());
            else
                this.publishDebug(this.status, this.position, car.getModel(), request.percentage());


            if (request.slot_id() != this.getPosition()) {
                car = null;
                this.setStatus(MWbotStatus.MOVING);
                this.publishDebug(this.status, this.position, null, request.percentage());
                this.moveToNewPosition(request.slot_id());
                this.setStatus(MWbotStatus.IDLE);
                this.publishDebug(this.status, this.position, null, request.percentage());
            }

            try {
                Thread.sleep(2000);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }

            car = ModelCar.DB.get(new Random().nextInt(ModelCar.DB.size()));
            this.publishDebug(this.status, this.position, car.getModel(), request.percentage());

            Integer kwCharged = this.chargeCarToPercentage(car, request);

            this.publish(request, kwCharged);

            request = null;
            this.publishDebug(this.status, this.position, car.getModel(), null);
        }
    }

    private MWbotStatus getStatus() {
        return this.status;
    }

    private Integer getPosition() {
        return this.position;
    }

    // Boolean getDebugLog() {
    //     return this.mqttClient.getDebugLog();
    // }

    private Boolean queueIsEmpty() {
        return this.queue.isEmpty();
    }

    private void setStatus(MWbotStatus status) {
        this.status = status;
    }

    private void setPosition(Integer position) {
        this.position = position;
    }

    private MWbotRequest getNewRequest() {
        return this.queue.removeFirst();
    }

    void addNewRequest(MWbotRequest request) {
        this.queue.addLast(request);
    }

    private void moveToNewPosition(Integer newPosition) {
        this.setStatus(MWbotStatus.MOVING);

        try {
            Thread.sleep(2000 * (newPosition - this.getPosition()) * (newPosition > this.getPosition() ? 1 : -1));
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

        this.setPosition(newPosition);
    }

    private Integer chargeCarToPercentage(ModelCar car, MWbotRequest request) {
        if (car.getKw() == null)
            return 0;

        Integer kwMax = car.getKw();
        Integer kwInit = new Random().nextInt(kwMax);
        Integer percentageInit = (kwInit * 100) / kwMax; // x / 100 = kwInit / kwMax

        if (percentageInit >= request.percentage())
            return 0;

        this.setStatus(MWbotStatus.CHARGING);
        this.publishDebug(this.status, this.position, car.getModel(), request.percentage());

        Integer chargeTime = (kwMax - kwInit) / MWbot.CHARGE_KW_PER_SEC;
        try {
            Thread.sleep(chargeTime * 1000);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

        this.setStatus(MWbotStatus.IDLE);
        this.publishDebug(this.status, this.position, car.getModel(), request.percentage());

        return kwMax - kwInit;
    }

    private void publishDebug(MWbotStatus status, int position, String model, Integer percentage) {
        if (model == null)
            model = "";

        if (percentage == null)
            percentage = 0;

        String message = JsonWrapper
                .toJson(new MwBotDataDebug(status, position, model, percentage));

        this.mqttClient.publishToTopic(TOPICS.MWBOT.getDebug(), message);
    }

    private void publish(MWbotRequest request, Integer kw) {
        String message = JsonWrapper.toJson(new MWbotResponse(request.slot_id(), kw));

        // to backend
        this.mqttClient.publishToTopic(TOPICS.MWBOT.getAck(), message);

        if (request.phone_number().equals(""))
            return;

        // to mobile, if exist
        message = JsonWrapper.toJson(new MobileNotification("Car charged to " + request.percentage() + "% in slot " + request.slot_id() + " with " + kw + "KW"));

        this.mqttClient.publishToTopic(TOPICS.MOBILE.getNotify(request.phone_number()), message);
    }

    public void publish(String topic, int size) {
        this.mqttClient.publishToTopic(topic, String.valueOf(size));
    }

    private void close() {
        this.mqttClient.close();
    }

    public static void main(String[] args) {
        Boolean debugLog;

        Scanner scanner = new Scanner(System.in);
        if (args.length == 1) {
            debugLog = Boolean.parseBoolean(args[0]);
        } else {
            do {
                System.out.print("Insert 'debugLog': \n> ");
                debugLog = Boolean.parseBoolean(scanner.nextLine());
            } while (debugLog == null);
        }

        MWbot mwbot = new MWbot(debugLog);

        mwbot.publishDebug(mwbot.getStatus(), mwbot.getPosition(), null, null); // publish the initial status

        System.out.println("Press [Q] to quit");
        String input = "";
        do {
            input = scanner.nextLine();
        } while (!input.equalsIgnoreCase("q"));

        scanner.close();
        mwbot.close();
    }

    // public void printDebug(String string) {
    //     this.mqttClient.printDebug(string);
    // }

    public ArrayList<MWbotRequest> getQueue() {
        return this.queue;
    }
}
