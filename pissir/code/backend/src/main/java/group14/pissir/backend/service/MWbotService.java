package group14.pissir.backend.service;

import group14.pissir.backend.model.RequestRepository;
import group14.pissir.backend.model.SlotRepository;

public class MWbotService {
    private static MWbotService instance = null;

    private MWbotService() {
        instance = this;
    }

    public static MWbotService getInstance() {
        if (instance == null)
            return new MWbotService();
        else
            return instance;
    }

    public void taskCompleted(Integer slotId, Integer kw) {
        if (SlotRepository.getInstance().getSlot(slotId) == null) {
            return;
        }

        RequestRepository.getInstance().updateRequestKW(slotId, kw);
    }

}
