package group14.pissir.iot.mwbot;

import group14.pissir.util.IMqttCallbackWrapper;
import group14.pissir.util.JsonWrapper;
import group14.pissir.util.TOPICS;
import group14.pissir.util.schema.MWbotRequest;

public class MWbotCallBack implements IMqttCallbackWrapper {

    private MWbot mwbot;

    MWbotCallBack(MWbot mwbot) {
        this.mwbot = mwbot;
    }

    @Override
    public void connectionLost(Throwable cause) {
        if (this.mwbot.mqttClient.getDebugLog())
			this.mwbot.mqttClient.printDebug("Connection lost, try to reconnect");

		try {
			this.mwbot.mqttClient.reconnect();
		} catch (Exception e) {
			this.mwbot.mqttClient.printError("Error while reconnecting: " + e.getMessage());
		}
    }

    @Override
    public void messageArrived(String topic, String message) throws Exception {
        if (this.mwbot.mqttClient.getDebugLog())
            this.mwbot.mqttClient.printDebug("Message arrived for the topic '" + topic + "': " + message);

        if (topic.equals(TOPICS.MWBOT.getNewRequest())) {
			MWbotRequest request = JsonWrapper.fromJson(message, MWbotRequest.class);
			this.mwbot.addNewRequest(request);
			this.mwbot.run();
            return;
		}

        if (topic.equals(TOPICS.getLwt())) {
			System.out.println("Get LWT message: " + message);
			return;
		}

        System.out.println("Uknown topic:" + topic);
    }

    @Override
    public void deliveryComplete(String token) {
        // Ho commentato il throw per evitare che il client chiuda la connessione
		// throw new UnsupportedOperationException(
		//         "Unimplemented method 'deliveryComplete'");
    }
}
