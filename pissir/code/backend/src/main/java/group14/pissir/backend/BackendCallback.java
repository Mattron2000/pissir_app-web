package group14.pissir.backend;

import group14.pissir.util.IMqttCallbackWrapper;
import group14.pissir.util.JsonWrapper;
import group14.pissir.util.MqttUtil;
import group14.pissir.util.TOPICS;
import group14.pissir.util.schema.MWbotResponse;

public class BackendCallback implements IMqttCallbackWrapper {

    private Backend backend;

    public BackendCallback(Backend backend) {
        this.backend = backend;
    }

    @Override
    public void connectionLost(Throwable cause) {
        if (this.backend.mqttClient.getDebugLog())
			this.backend.mqttClient.printDebug("Connection lost, try to reconnect");

		try {
			this.backend.mqttClient.reconnect();
		} catch (Exception e) {
			this.backend.mqttClient.printError("Error while reconnecting: " + e.getMessage());
		}
    }

    @Override
    public void messageArrived(String topic, String message) throws Exception {
        if (this.backend.mqttClient.getDebugLog())
            this.backend.mqttClient.printDebug("Message arrived for the topic '" + topic + "': " + message);

        if (topic.matches(TOPICS.SENSOR.getStatus(MqttUtil.Sensor.idPrefix + "\\d+"))) {
            this.backend.SensorMessage(message);
            return;
        }

        if (topic.equals(TOPICS.MWBOT.getAck())) {
            MWbotResponse ack = JsonWrapper.fromJson(message, MWbotResponse.class);

            this.backend.MwbotMessage(ack);
            return;
        }

        if (topic.equals(TOPICS.getLwt())) {
            System.out.println("Get LWT message: " + message);
            return;
        }

        if (this.backend.mqttClient.getDebugLog())
            this.backend.mqttClient.printDebug("Unkown message arrived for the topic '" + topic + "' with message: " + message);
    }

    @Override
    public void deliveryComplete(String token) {
        // throw new UnsupportedOperationException("Unimplemented method 'deliveryComplete'");
    }

}
