package group14.pissir.iot.mobile;

import java.util.ArrayList;
import java.util.Scanner;
import java.util.regex.Pattern;

import group14.pissir.util.IotDeviceType;
import group14.pissir.util.JsonWrapper;
import group14.pissir.util.MqttClientWrapper;
import group14.pissir.util.MqttUtil.Sensor;
import group14.pissir.util.schema.MobileData;
import group14.pissir.util.TOPICS;

public class Mobile {

	MqttClientWrapper mqttClient;

	private String message;

	private Mobile(String id, boolean debugLog) {
		this.message = "";

		ArrayList<String> subscribeTopics = new ArrayList<>();
		subscribeTopics.add(TOPICS.MOBILE.getNotify(id));
		subscribeTopics.add(TOPICS.MOBILE.getClose(id));

		this.mqttClient = new MqttClientWrapper(id, IotDeviceType.MOBILE, debugLog, subscribeTopics,
				new MobileCallBack(this));
	}

	// String getId() {
	// 	return this.mqttClient.getId();
	// }

	// boolean getDebugLog() {
	// 	return this.mqttClient.getDebugLog();
	// }

	void setMessage(String message) {
		this.message = message;
	}

	void publish() {
		String message = JsonWrapper.toJson(new MobileData(this.mqttClient.getId(), this.message));

		this.mqttClient.publishToTopic(TOPICS.MOBILE.getData(this.mqttClient.getId()), message);
	}

	void close() {
		this.mqttClient.close();
	}

	public static void main(String[] args) {
		String phoneNumber;
		Boolean debugLog;

		Scanner scanner = new Scanner(System.in);
		if (args.length == 2) {
			try {
				phoneNumber = args[0];
				debugLog = Boolean.parseBoolean(args[1]);
			} catch (ArrayIndexOutOfBoundsException | IllegalArgumentException e) {
				System.err.println("Usage: java " + Sensor.class.getName()
						+ " <phoneNumber> <true|false>");
				scanner.close();
				return;
			}
		} else {
			Pattern pattern = Pattern.compile("^(\\d{3}[- .]?){2}\\d{4}$");

			do {
				phoneNumber = null;
				debugLog = null;

				System.out.print("Insert 'phoneNumber': \n> ");
				phoneNumber = scanner.nextLine();
				if (!pattern.matcher(phoneNumber).matches()) {
					System.err.println("Invalid phoneNumber");
					continue;
				}

				System.out.print("Insert 'debugLog': \n> ");
				debugLog = Boolean.parseBoolean(scanner.nextLine());
			} while (phoneNumber == null || debugLog == null);
		}

		Pattern pattern = Pattern.compile("^\\d{10}$");
		if (!pattern.matcher(phoneNumber).matches()) {
			phoneNumber = phoneNumber.replace("-", "");
			phoneNumber = phoneNumber.replace(" ", "");
		}

		Mobile mobile = new Mobile(phoneNumber, debugLog);

		mobile.publish(); // publish the initial status

		String input;
		System.out.println("Press [Q] to quit");
		do {
			input = null;
			input = scanner.nextLine();

			if (input.equals("Q") || input.equals("q"))
				break;
		} while (true);

		scanner.close();
		mobile.close();
	}

    // public void printDebug(String string) {
	// 	this.mqttClient.printDebug(string);
    // }
}
