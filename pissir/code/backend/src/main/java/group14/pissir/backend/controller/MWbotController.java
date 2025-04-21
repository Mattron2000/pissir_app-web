package group14.pissir.backend.controller;

import group14.pissir.backend.service.MWbotService;
import group14.pissir.backend.service.QueueService;

public class MWbotController {

    private static MWbotController instance = null;

    public static MWbotController getInstance() {
        if (instance == null)
            return new MWbotController();
        else
            return instance;
    }

    public MWbotController() {
        instance = this;
    }

    public void manageAck(int slotId, int kw) {
        QueueService.getInstance().deleteQueue(slotId);
        MWbotService.getInstance().taskCompleted(slotId, kw);
    }
}
