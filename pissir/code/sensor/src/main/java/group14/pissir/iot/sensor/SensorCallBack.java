package group14.pissir.iot.sensor;

import group14.pissir.util.IMqttCallbackWrapper;
import group14.pissir.util.SensorStatus;
import group14.pissir.util.TOPICS;

class SensorCallBack implements IMqttCallbackWrapper {

	private Sensor sensor;

	SensorCallBack(Sensor sensor) {
		this.sensor = sensor;
	}

	@Override
	public void connectionLost(Throwable cause) {
		if (this.sensor.mqttClient.getDebugLog())
			this.sensor.mqttClient.printDebug("Connection lost, try to reconnect");

		try {
			this.sensor.mqttClient.reconnect();
		} catch (Exception e) {
			this.sensor.mqttClient.printError("Error while reconnecting: " + e.getMessage());
		}
	}

	@Override
	public void messageArrived(String topic, String message) throws Exception {
		if (this.sensor.mqttClient.getDebugLog())
			this.sensor.mqttClient.printDebug("Message arrived for the topic '" + topic + "': " + message);

		if (topic.equals(TOPICS.SENSOR.getCheck(this.sensor.mqttClient.getId()))) {
			this.sensor.publish();
			return;
		}

		if (topic.equals(TOPICS.SENSOR.getSwitch(this.sensor.mqttClient.getId()))) {
			if (this.sensor.getStatus() == SensorStatus.FREE)
				this.sensor.setStatus(SensorStatus.OCCUPIED);
			else
				this.sensor.setStatus(SensorStatus.FREE);

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
