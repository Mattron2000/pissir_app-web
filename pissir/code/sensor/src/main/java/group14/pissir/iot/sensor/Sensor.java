package group14.pissir.iot.sensor;

import java.util.ArrayList;
import java.util.Scanner;

import group14.pissir.util.IotDeviceType;
import group14.pissir.util.JsonWrapper;
import group14.pissir.util.MqttClientWrapper;
import group14.pissir.util.MqttUtil;
import group14.pissir.util.SensorStatus;
import group14.pissir.util.TOPICS;
import group14.pissir.util.schema.SensorData;

class Sensor {
	MqttClientWrapper mqttClient;
	private SensorStatus status;
	private Integer slotId;

	private Sensor(Integer slotId, SensorStatus status, Boolean debugLog) {
		this.status = status;
		this.slotId = slotId;

		ArrayList<String> subscribeTopics = new ArrayList<>();
		subscribeTopics.add(TOPICS.SENSOR.getCheck(MqttUtil.Sensor.idPrefix + this.slotId));
		subscribeTopics.add(TOPICS.SENSOR.getSwitch(MqttUtil.Sensor.idPrefix + this.slotId));

		this.mqttClient = new MqttClientWrapper(this.slotId.toString(), IotDeviceType.SENSOR, debugLog, subscribeTopics,
				new SensorCallBack(this));
	}

	// String getId() {
	// 	return this.mqttClient.getId();
	// }

	// boolean getDebugLog() {
	// 	return this.mqttClient.getDebugLog();
	// }

	SensorStatus getStatus() {
		return status;
	}

	void setStatus(SensorStatus stato) {
		if (stato == null || this.status == stato)
			return;

		this.status = stato;

		publish();
	}

	void publish() {
		String message = JsonWrapper
				.toJson(new SensorData(this.slotId, this.status));

		this.mqttClient.publishToTopic(TOPICS.SENSOR.getStatus(this.mqttClient.getId()), message);
	}

	private void close() {
		this.mqttClient.close();
	}

	public static void main(String[] args) {
		Integer postoId = null;
		SensorStatus stato = null;
		Boolean debugLog = null;

		System.out.println(args.length == 3);

		if (args.length == 3) {
			System.out.println("Sensor started, args: " + args[0] + " " + args[1] + " " + args[2]);
			postoId = Integer.parseInt(args[0]);
			stato = SensorStatus.valueOf(args[1]);
			debugLog = Boolean.parseBoolean(args[2]);
		} else {
			postoId = 1;
			stato = SensorStatus.FREE;
			debugLog = true;
		}

		System.out.println("Sensor " + postoId + " started with status " + stato + " and debugLog " + debugLog);

		Sensor sensor = new Sensor(postoId, stato, debugLog);

		sensor.publish(); // publish the initial status

		Scanner scanner = new Scanner(System.in);

		String input;
		System.out.println("To change status press [1] LIBERO, [2] OCCUPATO, [Q] EXIT");
		do {
			input = null;
			input = scanner.nextLine();

			if (input.equals("Q") || input.equals("q"))
				break;
			if (!input.equals("1") && !input.equals("2"))
				continue;

			stato = input.equals("1") ? SensorStatus.FREE
					: SensorStatus.OCCUPIED;
			sensor.setStatus(stato);
		} while (true);

		scanner.close();
		sensor.close();
	}
}
