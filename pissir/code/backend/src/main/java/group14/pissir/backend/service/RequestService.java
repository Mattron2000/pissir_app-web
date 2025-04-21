package group14.pissir.backend.service;

import java.util.ArrayList;

import group14.pissir.backend.model.RequestRepository;
import group14.pissir.util.schema.RequestData;
import group14.pissir.util.schema.RequestTimeData;

public class RequestService {

    private static RequestService instance = null;

    private RequestService() {
        instance = this;
    }

    public static RequestService getInstance() {
        if (instance == null)
            return new RequestService();
        else
            return instance;
    }

    public ArrayList<RequestTimeData> getWaitingTime() {
        return RequestRepository.getInstance().getWaitingTime();
    }

    public boolean updatePayment(String email) {
        return RequestRepository.getInstance().updateRequest(email);
    }

    public RequestData getUserUnpaidRequest(String email) {
        return RequestRepository.getInstance().getUserUnpaidRequest(email);
    }

    public boolean AddRequest(String email, String datetime_start, String datetime_end, Integer kw, boolean paid,
            int slot_id) {
        return RequestRepository.getInstance().addRequest(email, datetime_start, datetime_end, kw, paid, slot_id);
    }

    public ArrayList<RequestData> getUserUnpaidRequests(String email) {
        return RequestRepository.getInstance().getUserUnpaidRequests(email);
    }
}
