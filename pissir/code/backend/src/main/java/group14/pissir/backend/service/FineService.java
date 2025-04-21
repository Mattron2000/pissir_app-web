package group14.pissir.backend.service;

import java.time.LocalDateTime;
import java.util.ArrayList;

import group14.pissir.backend.model.FineRepository;
import group14.pissir.util.schema.DatetimesPaidData;
import group14.pissir.util.schema.EmailDatetimesData;
import group14.pissir.util.schema.EmailDatetimesPaidData;

public class FineService {

    private static FineService instance = null;

    private FineService() {
        instance = this;
    }

    public static FineService getInstance() {
        if (instance == null)
            return new FineService();
        else
            return instance;
    }

    public ArrayList<EmailDatetimesPaidData> getFines() {
        return FineRepository.getInstance().getFines();
    }

    public ArrayList<DatetimesPaidData> getUserFines(String email) {
        return FineRepository.getInstance().getUserFines(email);
    }

    public boolean addFine(String email, LocalDateTime datetime_start, LocalDateTime datetime_end) {
        return FineRepository.getInstance().addFine(email, datetime_start, datetime_end);
    }

    public boolean updateFine(String email, String localDateTime) {
        return FineRepository.getInstance().updateFine(email, localDateTime);
    }

    public boolean addFine(EmailDatetimesData reservation) {
        return FineRepository.getInstance().addFine(reservation.email(), reservation.datetime_start(), reservation.datetime_end());
    }

    public boolean addFine(String email, String datetime_start, String datetime_end) {
        return FineRepository.getInstance().addFine(email, datetime_start, datetime_end);
    }

}
