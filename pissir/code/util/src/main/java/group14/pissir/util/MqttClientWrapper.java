package group14.pissir.util;

import java.util.ArrayList;

import org.eclipse.paho.client.mqttv3.MqttClient;
import org.eclipse.paho.client.mqttv3.MqttConnectOptions;
import org.eclipse.paho.client.mqttv3.MqttException;
import org.eclipse.paho.client.mqttv3.MqttMessage;
import org.eclipse.paho.client.mqttv3.MqttTopic;

import group14.pissir.util.schema.LwtData;

public class MqttClientWrapper {

    private final String id;
	private final IotDeviceType type;
	private final Boolean debugLog;
	private final String logPrefix;
	private MqttClient client;
	private MqttConnectOptions options;
	private ArrayList<String> subscribeTopics;

    public MqttClientWrapper(
            String id, IotDeviceType type, Boolean debugLog,
            ArrayList<String> subscribeTopics, IMqttCallbackWrapper callback) {
        this.options = new MqttConnectOptions();
		this.subscribeTopics = subscribeTopics;
		this.debugLog = debugLog;
		this.type = type;

        this.id = this.setupId(id);
        this.setUsernameAndPassword();
        this.logPrefix = "[" + this.type + "] id='" + this.id + "' : ";

		this.options.setCleanSession(true);
		this.options.setAutomaticReconnect(true);

        try {
			this.client = new MqttClient(URLS.BROKER_URL, this.id);
			this.client.connect(this.options);

			for (String topic : this.subscribeTopics)
				this.client.subscribe(topic);
		} catch (MqttException e) {
			e.printStackTrace();
		}

		this.setCallBack(callback);

        this.setWill(new LwtData(this.id, "I'm gone. Bye."));

        if (this.debugLog) {
			printDebug("MQTT client created.");
			for (String topic : this.subscribeTopics)
				printDebug("Listening to topic: " + topic);
		}
    }

	public String getId() {
		return id;
	}

    public Boolean getDebugLog() {
        return debugLog;
    }

    private String setupId(String id) {
		switch (this.type) {
			case MOBILE:
			case MWBOT:
			case MONITOR:
			case BACKEND:
				return id;
			case SENSOR:
				return MqttUtil.Sensor.idPrefix + id;
		}

		throw new RuntimeException("Unknown device type:" + type);
	}

    private void setUsernameAndPassword() {
		switch (type) {
			case MOBILE:
                this.options.setUserName(MqttUtil.Mobile.username);
                this.options.setPassword(MqttUtil.Mobile.password.toCharArray());
				break;
			case MONITOR:
                this.options.setUserName(MqttUtil.Monitor.username);
                this.options.setPassword(MqttUtil.Monitor.password.toCharArray());
				break;
			case SENSOR:
                this.options.setUserName(MqttUtil.Sensor.username);
                this.options.setPassword(MqttUtil.Sensor.password.toCharArray());
				break;
			case MWBOT:
                this.options.setUserName(MqttUtil.MWbot.username);
                this.options.setPassword(MqttUtil.MWbot.password.toCharArray());
				break;
			case BACKEND:
				this.options.setUserName(MqttUtil.Backend.username);
				this.options.setPassword(MqttUtil.Backend.password.toCharArray());
				break;
			default:
				throw new RuntimeException("Unknown device type:" + type);
		}
	}

    private void setWill(LwtData body) {
		this.options.setWill(this.client.getTopic(TOPICS.getLwt()), JsonWrapper.toJson(body).getBytes(), 0, false);
	}

    public void printDebug(String message) {
        System.out.println(this.logPrefix + message);
    }

    private void setCallBack(IMqttCallbackWrapper callback) {
		if (callback == null)
			return;

		this.client.setCallback(new MqttCallbackWrapper(callback));
	}

    public void publishToTopic(String topic, String message) {
        try {
			MqttTopic publishTopic = this.client.getTopic(topic);
			// publish the message on the given topic
			publishTopic.publish(new MqttMessage(message.getBytes()));
		} catch (MqttException e) {
			e.printStackTrace();
		}

		if (this.debugLog)
            this.printDebug("published message on topic '" + topic + "': " + message);
    }

    public void close() {
        try {
			if (this.client.isConnected())
				this.client.disconnect();
			this.client.close();
		} catch (MqttException e) {
			e.printStackTrace();
		}
    }

    public void reconnect() {
		try {
			this.client.connect();
		} catch (MqttException e) {
			e.printStackTrace();
		}
    }

    public void printError(String string) {
		System.err.println(this.logPrefix + string);
    }
}
