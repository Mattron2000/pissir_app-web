package group14.pissir.backend.model;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;

import group14.pissir.backend.db.DBConnect;
import group14.pissir.util.schema.ServicePriceData;

public class PriceRepository {

    private static PriceRepository instance = null;

    public static PriceRepository getInstance() {
        if (instance == null)
            return new PriceRepository();
        else
            return instance;
    }

    public ArrayList<ServicePriceData> getPrices() {
        final String sql = "SELECT * FROM prices";

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            ResultSet resultQuery = st.executeQuery();

            ArrayList<ServicePriceData> pricesData = new ArrayList<>();

            while (resultQuery.next())
                pricesData.add(new ServicePriceData(resultQuery.getString("type"), resultQuery.getFloat("price")));

            st.close();
            conn.close();

            return pricesData;
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return null;
    }

}
