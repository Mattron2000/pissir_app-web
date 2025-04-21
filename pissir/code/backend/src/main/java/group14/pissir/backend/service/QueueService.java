package group14.pissir.backend.service;

import group14.pissir.backend.model.QueueRepository;

public class QueueService {

    private static QueueService instance = null;

    public static QueueService getInstance() {
        if (instance == null)
            return new QueueService();
        else
            return instance;
    }

    public QueueService() {
        instance = this;
    }

    public int getQueueSize() {
        return QueueRepository.getInstance().getQueueSize();
    }

    public void addQueue(int slot_id, Integer percentage, String phone_number) {
        QueueRepository.getInstance().addQueue(slot_id, percentage, phone_number);
    }

    public void deleteQueue(Integer slotId) {
        QueueRepository.getInstance().deleteQueue(slotId);
    }
}
