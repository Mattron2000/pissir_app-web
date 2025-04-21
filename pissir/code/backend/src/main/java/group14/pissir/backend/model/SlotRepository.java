package group14.pissir.backend.model;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;

import group14.pissir.backend.db.DBConnect;
import group14.pissir.util.SensorStatus;
import group14.pissir.util.schema.SensorData;

public class SlotRepository {

    private static SlotRepository instance = null;

    public static SlotRepository getInstance() {
        if (instance == null) {
            instance = new SlotRepository();
        }
        return instance;
    }

    private SlotRepository() {
        instance = this;
    }

    public ArrayList<Integer> getFreeSlots(String datetime_start, String datetime_end) {
        final String sql = """
            SELECT id
            FROM slots
            WHERE id NOT IN (
                SELECT slot_id
                FROM reservations
                WHERE datetime(datetime_start) BETWEEN datetime(?) AND datetime(?) OR
                datetime(datetime_end) BETWEEN datetime(?) AND datetime(?) OR
                datetime(?) BETWEEN datetime(datetime_start) AND datetime(datetime_end) OR
                datetime(?) BETWEEN datetime(datetime_start) AND datetime(datetime_end)
                ) AND id NOT IN (
                    SELECT slot_id
                    FROM requests
                    WHERE datetime(datetime_start) BETWEEN datetime(?) AND datetime(?) OR
                    datetime(datetime_end) BETWEEN datetime(?) AND datetime(?) OR
                    datetime(?) BETWEEN datetime(datetime_start) AND datetime(datetime_end) OR
                    datetime(?) BETWEEN datetime(datetime_start) AND datetime(datetime_end)
                )
                AND status = 'FREE'
            """;

        ArrayList<Integer> ids = new ArrayList<>();

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, datetime_start);
            st.setString(2, datetime_end);
            st.setString(3, datetime_start);
            st.setString(4, datetime_end);
            st.setString(5, datetime_start);
            st.setString(6, datetime_end);
            st.setString(7, datetime_start);
            st.setString(8, datetime_end);
            st.setString(9, datetime_start);
            st.setString(10, datetime_end);
            st.setString(11, datetime_start);
            st.setString(12, datetime_end);

            ResultSet resultQuery = st.executeQuery();

            while (resultQuery.next())
                ids.add(resultQuery.getInt("id"));

            st.close();
            conn.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return ids;
    }

    public boolean setSlot(Integer id, SensorStatus status) {
        final String sql = "UPDATE slots SET status = ? WHERE id = ?";

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, status.toString());
            st.setInt(2, id);

            int rowsUpdated = st.executeUpdate();

            st.close();
            conn.close();

            if (rowsUpdated != 1)
                System.out.println("ERROR: slot was not updated: rowUpdated = " + rowsUpdated);

            return rowsUpdated == 1;
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return false;
    }

    public SensorData getSlot(Integer slot_id) {
        final String sql = "SELECT * FROM slots WHERE id = ?";

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setInt(1, slot_id);

            ResultSet resultQuery = st.executeQuery();

            SensorData sensorData = null;
            if (resultQuery.next())
                sensorData = new SensorData(resultQuery.getInt("id"), SensorStatus.valueOf(resultQuery.getString("status")));

            st.close();
            conn.close();

            return sensorData;
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return null;
    }

    public ArrayList<SensorData> getSlots() {
        final String sql = "SELECT * FROM slots";

        ArrayList<SensorData> slots = new ArrayList<>();

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            ResultSet resultQuery = st.executeQuery();

            while (resultQuery.next())
                slots.add(new SensorData(resultQuery.getInt("id"), SensorStatus.valueOf(resultQuery.getString("status"))));

            st.close();
            conn.close();

            return slots;
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return null;
    }
}
