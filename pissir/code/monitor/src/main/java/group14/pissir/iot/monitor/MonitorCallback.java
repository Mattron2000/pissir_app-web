package group14.pissir.iot.monitor;

import group14.pissir.util.IMqttCallbackWrapper;
import group14.pissir.util.JsonWrapper;
import group14.pissir.util.MqttUtil;
import group14.pissir.util.TOPICS;
import group14.pissir.util.schema.SensorData;

public class MonitorCallback implements IMqttCallbackWrapper {

	private Monitor monitor;

	MonitorCallback(Monitor monitor) {
		this.monitor = monitor;
	}

	@Override
	public void connectionLost(Throwable cause) {
		// Ho commentato il throw per evitare che il client chiuda la connessione
		// throw new UnsupportedOperationException("Unimplemented method 'connectionLost'");

		if (this.monitor.mqttClient.getDebugLog())
			this.monitor.mqttClient.printDebug("Connection lost, try to reconnect");

		try {
			this.monitor.mqttClient.reconnect();
		} catch (Exception e) {
			this.monitor.mqttClient.printError("Error while reconnecting: " + e.getMessage());
		}
	}

	@Override
	public void messageArrived(String topic, String message) throws Exception {
		if (this.monitor.mqttClient.getDebugLog())
			this.monitor.mqttClient.printDebug("Message arrived for the topic '" + topic + "': " + message);

		if (topic.equals(TOPICS.MONITOR.getCheck())) {
			this.monitor.publish();
			return;
		}

		if (topic.matches(TOPICS.SENSOR.getStatus(MqttUtil.Sensor.idPrefix + "\\d+"))) {
			SensorData sensorData = JsonWrapper.fromJson(message, SensorData.class);
			this.monitor.updateCount(sensorData);
			return;
		}

		if (topic.equals(TOPICS.getLwt())) {
			System.out.println("Get LWT message: " + message);
			return;
		}
	}

	@Override
	public void deliveryComplete(String token) {
		// Ho commentato il throw per evitare che il client chiuda la connessione
		// throw new UnsupportedOperationException(
		//         "Unimplemented method 'deliveryComplete'");
	}

}
