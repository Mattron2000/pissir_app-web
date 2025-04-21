package group14.pissir.util;

import java.util.ArrayList;

public class ModelCar {

    public static final ArrayList<ModelCar> DB = new ArrayList<>();

    static {
        DB.add(new ModelCar("500e", 24));
        DB.add(new ModelCar("Model S", 95));
        DB.add(new ModelCar("Spring", 26));
        DB.add(new ModelCar("e-208", 50));
        DB.add(new ModelCar("Panda 4x4", null));
        DB.add(new ModelCar("C3", null));
    }

    private String model;
    private Integer kw;

    public ModelCar(String model, Integer kw) {
        this.model = model;
        this.kw = kw;
    }

    public Integer getKw() {
        return kw;
    }

    public String getModel() {
        return model;
    }
}
