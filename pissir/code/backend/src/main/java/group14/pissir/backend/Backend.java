package group14.pissir.backend;

import java.util.ArrayList;
import java.util.Scanner;

import group14.pissir.backend.controller.MWbotController;
import group14.pissir.backend.controller.SlotController;
import group14.pissir.util.IotDeviceType;
import group14.pissir.util.MqttClientWrapper;
import group14.pissir.util.MqttUtil;
import group14.pissir.util.TOPICS;
import group14.pissir.util.schema.EmailDatetimesData;
import group14.pissir.util.schema.MWbotResponse;
import io.javalin.Javalin;
import io.javalin.util.JavalinLogger;

public class Backend {

	private static Backend instance = null;

	private static Integer port = 9999;

	MqttClientWrapper mqttClient;
	private Javalin javalin;
	private ReservationChecker reservationChecker;

	public static Backend getInstance() {
		if (instance == null)
			return new Backend();
		else
			return instance;
	}

	public Backend() {
		instance = this;

		this.javalin = startJavalin();
		this.mqttClient = startMqttClient();
		this.reservationChecker = startReservationCheck();
	}

	private ReservationChecker startReservationCheck() {
		ReservationChecker reservationChecker = ReservationChecker.getInstance();

		return reservationChecker;
	}

	private void close() {
		javalin.stop();
		mqttClient.close();
	}

	private MqttClientWrapper startMqttClient() {
		ArrayList<String> subscribeTopics = new ArrayList<>();

		subscribeTopics.add(TOPICS.SENSOR.getStatus("+"));
		subscribeTopics.add(TOPICS.MWBOT.getAck());

		return new MqttClientWrapper(MqttUtil.Backend.id, IotDeviceType.BACKEND, true,
				subscribeTopics, new BackendCallback(this));
	}

	private Javalin startJavalin() {
		JavalinLogger.info("Swagger docs at http://localhost:" + port + "/swagger");
		Javalin javalin = Javalin.create(JavalinBackendConfig.getConfig());

		JavalinLogger.info("Starting server on http://localhost:" + port);
		return javalin.start(port);
	}

	// public boolean getDebugLog() {
	// 	return this.mqttClient.getDebugLog();
	// }

	// public void printDebug(String message) {
	// 	this.mqttClient.printDebug(message);
	// }

	public void SensorMessage(String message) {
		SlotController.getInstance().setSlot(message);
	}

	public void MwbotMessage(MWbotResponse ack) {
		MWbotController.getInstance().manageAck(ack.slotId(), ack.kw());
	}

	public static void main(String[] args) {
		Scanner scanner = new Scanner(System.in);

		Backend backend = Backend.getInstance();

				System.out.println("Press [Q] to quit");
				String input;
				do {
					input = scanner.nextLine();

					if (input.equals("Q") || input.equals("q"))
						break;
				} while (true);

				scanner.close();
				backend.close();
			}

	public void publish(String topic, String message) {
		this.mqttClient.publishToTopic(topic, message);
	}

	public void schedule(EmailDatetimesData reservation) {
		this.reservationChecker.schedule(reservation);
	}

	public void schedule(String email, String datetime_start, String datetime_end) {
		this.reservationChecker.schedule(email, datetime_start, datetime_end);
	}

    public boolean getDebugLog() {
		return this.mqttClient.getDebugLog();
	}
}
