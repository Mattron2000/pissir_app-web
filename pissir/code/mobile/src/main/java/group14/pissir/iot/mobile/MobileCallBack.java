package group14.pissir.iot.mobile;

import group14.pissir.util.IMqttCallbackWrapper;
import group14.pissir.util.JsonWrapper;
import group14.pissir.util.TOPICS;
import group14.pissir.util.schema.MobileNotification;

public class MobileCallBack implements IMqttCallbackWrapper {

	private Mobile mobile;

	public MobileCallBack(Mobile mobile) {
		this.mobile = mobile;
	}

	@Override
	public void connectionLost(Throwable cause) {
		if (this.mobile.mqttClient.getDebugLog())
			this.mobile.mqttClient.printDebug("Connection lost, try to reconnect");

		try {
			this.mobile.mqttClient.reconnect();
		} catch (Exception e) {
			this.mobile.mqttClient.printError("Error while reconnecting: " + e.getMessage());
		}
	}

	@Override
	public void messageArrived(String topic, String message) throws Exception {
		if (this.mobile.mqttClient.getDebugLog())
			this.mobile.mqttClient.printDebug("Message arrived for the topic '" + topic + "': " + message);

		if (topic.equals(TOPICS.MOBILE.getNotify(this.mobile.mqttClient.getId()))) {
			MobileNotification notification = JsonWrapper.fromJson(message, MobileNotification.class);
			this.mobile.setMessage(notification.message());
			this.mobile.publish();
			return;
		}

		if (topic.equals(TOPICS.MOBILE.getClose(this.mobile.mqttClient.getId()))) {
			System.out.println("Mobile closed");
			this.mobile.close();
			System.exit(0);
			return;
		}

		if (topic.equals(TOPICS.getLwt())) {
			System.out.println("Get LWT message: " + message);
			return;
		}
	}

	@Override
	public void deliveryComplete(String token) {
		// ho commentato il throw per evitare che il client chiuda la connessione dopo un publish

		// throw new UnsupportedOperationException("Unimplemented method 'deliveryComplete'");
	}

}
