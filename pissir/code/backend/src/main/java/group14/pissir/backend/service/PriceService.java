package group14.pissir.backend.service;

import java.util.ArrayList;

import group14.pissir.backend.model.PriceRepository;
import group14.pissir.util.schema.ServicePriceData;

public class PriceService {

    private static PriceService instance = null;

    public static PriceService getInstance() {
        if (instance == null)
            return new PriceService();
        else
            return instance;
    }

    public PriceService() {
        instance = this;
    }

    public ArrayList<ServicePriceData> getPrices() {
        return PriceRepository.getInstance().getPrices();
    }

}
