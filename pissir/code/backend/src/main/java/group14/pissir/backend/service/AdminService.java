package group14.pissir.backend.service;

import java.time.LocalDate;
import java.time.LocalTime;
import java.util.ArrayList;

import group14.pissir.backend.model.AdminRespository;
import group14.pissir.util.schema.RequestData;

public class AdminService {

    private static AdminService instance = null;

    public static AdminService getInstance() {
        if (instance == null)
            return new AdminService();
        else
            return instance;
    }

    public AdminService() {
        instance = this;
    }

    public boolean setPrice(float price, String type) {
        return AdminRespository.getInstance().setPrice(price, type);
    }

    public ArrayList<RequestData> getRequestHistoryByData(LocalDate date_start, LocalDate date_end) {
        return AdminRespository.getInstance().getRequestHistoryByData(date_start, date_end);
    }

    public ArrayList<RequestData> getRequestHistoryByTime(LocalTime time_start, LocalTime time_end) {
        return AdminRespository.getInstance().getRequestHistoryByTime(time_start, time_end);
    }

    public ArrayList<RequestData> getRequestHistory() {
        return AdminRespository.getInstance().getRequestHistory();
    }

    public ArrayList<RequestData> getRequestHistoryByType(String type) {
        return AdminRespository.getInstance().getRequestHistoryByType(type);
    }

    public ArrayList<RequestData> getRequestHistoryByGrade(String grade) {
        return AdminRespository.getInstance().getRequestHistoryByGrade(grade);
    }
}
