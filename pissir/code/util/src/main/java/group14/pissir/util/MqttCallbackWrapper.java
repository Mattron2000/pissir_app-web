package group14.pissir.util;

import java.nio.charset.StandardCharsets;

import org.eclipse.paho.client.mqttv3.IMqttDeliveryToken;
import org.eclipse.paho.client.mqttv3.MqttCallback;
import org.eclipse.paho.client.mqttv3.MqttMessage;

public class MqttCallbackWrapper implements MqttCallback {
	private IMqttCallbackWrapper callback;

	public MqttCallbackWrapper(IMqttCallbackWrapper callback) {
		this.callback = callback;
	}

	@Override
	public void connectionLost(Throwable cause) {
		if (this.callback != null)
			this.callback.connectionLost(cause);
		else
			throw new UnsupportedOperationException(
					"Unimplemented method 'connectionLost'");
	}

	@Override
	public void messageArrived(String topic, MqttMessage mqttMessage)
			throws Exception {
		if (this.callback != null)
			this.callback.messageArrived(topic, new String(
					mqttMessage.getPayload(), StandardCharsets.UTF_8));
		else
			throw new UnsupportedOperationException(
					"Unimplemented method 'messageArrived'");
	}

	@Override
	public void deliveryComplete(IMqttDeliveryToken token) {
		if (this.callback != null)
			this.callback.deliveryComplete(token.toString());
		else
			throw new UnsupportedOperationException(
					"Unimplemented method 'deliveryComplete'");
	}
}
