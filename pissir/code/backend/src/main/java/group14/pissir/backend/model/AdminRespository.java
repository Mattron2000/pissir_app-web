package group14.pissir.backend.model;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.time.LocalDate;
import java.time.LocalTime;
import java.util.ArrayList;

import group14.pissir.backend.db.DBConnect;
import group14.pissir.util.schema.RequestData;

public class AdminRespository {

    private static AdminRespository instance = null;

    public static AdminRespository getInstance() {
        if (instance == null)
            return new AdminRespository();
        else
            return instance;
    }

    public AdminRespository() {
        instance = this;
    }

    public boolean setPrice(float price, String type) {
        final String sql = "UPDATE prices SET price = ? WHERE type = ?";

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setFloat(1, price);
            st.setString(2, type);

            int rowsUpdated = st.executeUpdate();

            st.close();
            conn.close();

            if (rowsUpdated == 1)
                return true;
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return false;
    }

    public ArrayList<RequestData> getRequestHistoryByData(LocalDate date_start, LocalDate date_end) {
        final String sql = """
                SELECT email, type, datetime_start, datetime_end, kw, slot_id
                FROM requests JOIN users USING (email)
                WHERE date(datetime_start) BETWEEN ? AND ? AND date(datetime_end) BETWEEN ? AND ? AND paid = true
                """;

        ArrayList<RequestData> requests = new ArrayList<>();

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, date_start.toString());
            st.setString(2, date_end.toString());
            st.setString(3, date_start.toString());
            st.setString(4, date_end.toString());

            ResultSet resultQuery = st.executeQuery();

            while (resultQuery.next()) {
                String email = resultQuery.getString("email");
                String type = resultQuery.getString("type");
                String datetime_start = resultQuery.getString("datetime_start");
                String datetime_end = resultQuery.getString("datetime_end");
                Integer kw = resultQuery.getInt("kw");
                if (resultQuery.wasNull()) kw = null;
                int slot_id = resultQuery.getInt("slot_id");
                requests.add(new RequestData(email, type, datetime_start, datetime_end, kw, slot_id));
            }

            st.close();
            conn.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return requests;
    }

    public ArrayList<RequestData> getRequestHistoryByTime(LocalTime time_start, LocalTime time_end) {
        final String sql = """
                SELECT email, type, datetime_start, datetime_end, kw, slot_id
                FROM requests JOIN users USING (email)
                WHERE (time(datetime_start) BETWEEN time(?) AND time(?) OR time(datetime_end) BETWEEN time(?) AND time(?)) AND paid = true
                """;

        ArrayList<RequestData> requests = new ArrayList<>();

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            ;

            st.setString(1, time_start.toString());
            st.setString(2, time_end.toString());
            st.setString(3, time_start.toString());
            st.setString(4, time_end.toString());

            ResultSet resultQuery = st.executeQuery();

            while (resultQuery.next()) {
                String email = resultQuery.getString("email");
                String type = resultQuery.getString("type");
                String datetime_start = resultQuery.getString("datetime_start");
                String datetime_end = resultQuery.getString("datetime_end");
                Integer kw = resultQuery.getInt("kw");
                if (resultQuery.wasNull()) kw = null;
                int slot_id = resultQuery.getInt("slot_id");
                requests.add(new RequestData(email, type, datetime_start, datetime_end, kw, slot_id));
            }

            st.close();
            conn.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return requests;
    }

    public ArrayList<RequestData> getRequestHistory() {
        final String sql = """
                SELECT email, type, datetime_start, datetime_end, kw, slot_id
                FROM requests JOIN users USING (email)
                WHERE paid = true
                """;

        ArrayList<RequestData> requests = new ArrayList<>();

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            ResultSet resultQuery = st.executeQuery();

            while (resultQuery.next()) {
                String email = resultQuery.getString("email");
                String type = resultQuery.getString("type");
                String datetime_start = resultQuery.getString("datetime_start");
                String datetime_end = resultQuery.getString("datetime_end");
                Integer kw = resultQuery.getInt("kw");
                if (resultQuery.wasNull()) kw = null;
                int slot_id = resultQuery.getInt("slot_id");
                requests.add(new RequestData(email, type, datetime_start, datetime_end, kw, slot_id));
            }

            st.close();
            conn.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return requests;
    }

    public ArrayList<RequestData> getRequestHistoryByType(String serviceType) {
        String sql = null;

        if (serviceType == null || !serviceType.equals("PARKING") && !serviceType.equals("CHARGING"))
            return null;

        if (serviceType.equals("PARKING"))
            sql = """
                SELECT email, type, datetime_start, datetime_end, kw, slot_id
                FROM requests JOIN users USING (email)
                WHERE kw IS NULL AND paid = true
            """;
        else
            sql = """
                SELECT email, type, datetime_start, datetime_end, kw, slot_id
                FROM requests JOIN users USING (email)
                WHERE kw IS NOT NULL AND paid = true
            """;

        ArrayList<RequestData> requests = new ArrayList<>();

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            ResultSet resultQuery = st.executeQuery();

            while (resultQuery.next()) {
                String email = resultQuery.getString("email");
                String type = resultQuery.getString("type");
                String datetime_start = resultQuery.getString("datetime_start");
                String datetime_end = resultQuery.getString("datetime_end");
                Integer kw = resultQuery.getInt("kw");
                if (resultQuery.wasNull()) kw = null;
                int slot_id = resultQuery.getInt("slot_id");
                requests.add(new RequestData(email, type, datetime_start, datetime_end, kw, slot_id));
            }

            st.close();
            conn.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return requests;
    }

    public ArrayList<RequestData> getRequestHistoryByGrade(String grade) {
        final String sql = """
                SELECT email, type, datetime_start, datetime_end, kw, slot_id
                FROM requests JOIN users USING (email)
                WHERE type = ? AND paid = true
                """;

        ArrayList<RequestData> requests = new ArrayList<>();

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, grade);

            ResultSet resultQuery = st.executeQuery();

            while (resultQuery.next()) {
                String email = resultQuery.getString("email");
                String type = resultQuery.getString("type");
                String datetime_start = resultQuery.getString("datetime_start");
                String datetime_end = resultQuery.getString("datetime_end");
                Integer kw = resultQuery.getInt("kw");
                if (resultQuery.wasNull()) kw = null;
                int slot_id = resultQuery.getInt("slot_id");
                requests.add(new RequestData(email, type, datetime_start, datetime_end, kw, slot_id));
            }

            st.close();
            conn.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return requests;
    }
}
