package group14.pissir.util;

public interface IMqttCallbackWrapper {
	public void connectionLost(Throwable cause);

	public void messageArrived(String topic, String message) throws Exception;

	public void deliveryComplete(String token);
}
