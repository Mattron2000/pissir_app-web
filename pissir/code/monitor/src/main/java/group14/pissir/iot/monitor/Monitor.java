package group14.pissir.iot.monitor;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Scanner;

import group14.pissir.util.IotDeviceType;
import group14.pissir.util.JsonWrapper;
import group14.pissir.util.MqttClientWrapper;
import group14.pissir.util.MqttUtil;
import group14.pissir.util.SensorStatus;
import group14.pissir.util.TOPICS;
import group14.pissir.util.schema.MonitorData;
import group14.pissir.util.schema.SensorData;

public class Monitor {

	MqttClientWrapper mqttClient;

	private HashMap<Integer, SensorStatus> sensors;

	private Monitor(boolean debugLog) {
		this.sensors = new HashMap<>();

		ArrayList<String> subscribeTopics = new ArrayList<>();
		subscribeTopics.add(TOPICS.SENSOR.getStatus("+"));
		subscribeTopics.add(TOPICS.MONITOR.getCheck());

		this.mqttClient = new MqttClientWrapper(MqttUtil.Monitor.id, IotDeviceType.MONITOR, debugLog, subscribeTopics,
				new MonitorCallback(this));
	}

	// private String getId() {
	// 	return this.mqttClient.getId();
	// }

	// boolean getDebugLog() {
	// 	return this.mqttClient.getDebugLog();
	// }

	void updateCount(SensorData sensorData) {
		if (sensorData == null)
			return;

		this.sensors.put(sensorData.slot_id(), sensorData.status());

		publish();
	}

	void publish() {
		long freeSlots = this.sensors.values().stream().filter(s -> s.equals(SensorStatus.FREE)).count();

		String message = JsonWrapper.toJson(new MonitorData(this.mqttClient.getId(), freeSlots));

		this.mqttClient.publishToTopic(TOPICS.MONITOR.getCount(), message);
	}

	private void close() {
		this.mqttClient.close();
	}

	public static void main(String[] args) {
		Boolean debugLog;

		Scanner scanner = new Scanner(System.in);

		if (args.length == 1)
			try {
				debugLog = Boolean.parseBoolean(args[0]);
			} catch (ArrayIndexOutOfBoundsException
					| IllegalArgumentException e) {
				System.err.println("Usage: java " + Monitor.class.getName() + " <true|false>");
				scanner.close();
				return;
			}
		else
			debugLog = true;

		Monitor monitor = new Monitor(debugLog);

		monitor.publish(); // publish the initial count of the monitor

		String input = null;
		System.out.println("Press [Q] to quit");
		do {
			input = scanner.nextLine();

			if (input.equals("Q") || input.equals("q"))
				break;
		} while (true);

		scanner.close();
		monitor.close();
	}

	public void printDebug(String string) {
		this.mqttClient.printDebug(string);
	}
}
