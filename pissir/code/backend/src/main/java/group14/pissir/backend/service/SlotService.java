package group14.pissir.backend.service;

import java.time.LocalDateTime;
import java.util.ArrayList;

import group14.pissir.backend.model.SlotRepository;
import group14.pissir.util.SensorStatus;
import group14.pissir.util.schema.SensorData;

public class SlotService {

    private static SlotService instance = null;

    public static SlotService getInstance() {
        if (instance == null)
            return new SlotService();
        else
            return instance;
    }

    public SlotService() {
        instance = this;
    }

    public Integer getFirstFreeSlot(LocalDateTime datetime_start, LocalDateTime datetime_end) {
        ArrayList<Integer> ids = SlotRepository.getInstance().getFreeSlots(datetime_start.toString(), datetime_end.toString());

        return ids.size() > 0 ? ids.get(0) : null;
    }

    public void setSlot(Integer slot_id, SensorStatus status) {
        if (SlotRepository.getInstance().getSlot(slot_id) == null)
            return;

        SlotRepository.getInstance().setSlot(slot_id, status);
    }

    public ArrayList<SensorData> getSlots() {
        return SlotRepository.getInstance().getSlots();
    }

}
