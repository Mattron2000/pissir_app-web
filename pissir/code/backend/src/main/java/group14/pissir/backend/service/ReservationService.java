package group14.pissir.backend.service;

import java.util.ArrayList;

import group14.pissir.backend.model.ReservationRepository;
import group14.pissir.util.schema.EmailDatetimesData;
import group14.pissir.util.schema.ReservationDatetimesSlotData;

public class ReservationService {

    private static ReservationService instance = null;

    public static ReservationService getInstance() {
        if (instance == null)
            return new ReservationService();
        else
            return instance;
    }

    public ReservationService() {
        instance = this;
    }

    public ArrayList<ReservationDatetimesSlotData> getUserReservations(String email) {
        return ReservationRepository.getInstance().getUserReservations(email);
    }

    public boolean addReservation(String email, int slot_id, String datetime_start, String datetime_end) {
        return ReservationRepository.getInstance().addReservation(email, slot_id, datetime_start, datetime_end);
    }

    public boolean deleteReservation(String email, String datetime_start) {
        return ReservationRepository.getInstance().deleteReservation(email, datetime_start);
    }

    public ArrayList<EmailDatetimesData> getReservations() {
        return ReservationRepository.getInstance().getReservations();
    }
}
