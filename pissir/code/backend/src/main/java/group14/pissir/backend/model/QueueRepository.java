package group14.pissir.backend.model;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import group14.pissir.backend.db.DBConnect;

public class QueueRepository {

    private static QueueRepository instance = null;

    public static QueueRepository getInstance() {
        if (instance == null)
            return new QueueRepository();
        else
            return instance;
    }

    public QueueRepository() {
        instance = this;
    }

    public int getQueueSize() {
        int queueSize = 0;

        final String sql = "SELECT COUNT(*) FROM queue";

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            ResultSet resultQuery = st.executeQuery();

            if (resultQuery.next())
                queueSize = resultQuery.getInt(1);

            st.close();
            conn.close();

            return queueSize;
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return queueSize;
    }

    public boolean addQueue(int slot_id, Integer percentage, String phone_number) {
        final String sql = "INSERT INTO queue (slot_id, percentage, phone_number) VALUES (?, ?, ?)";

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setInt(1, slot_id);
            st.setInt(2, percentage);
            st.setString(3, phone_number);

            int rowsInserted = st.executeUpdate();

            st.close();
            conn.close();

            if (rowsInserted == 1)
                return true;
            else
                return false;
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return false;
    }

    public void deleteQueue(Integer slotId) {
        final String sql = "DELETE FROM queue WHERE slot_id = ?";

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setInt(1, slotId);

            int rowsDeleted = st.executeUpdate();

            st.close();
            conn.close();

            if (rowsDeleted != 1)
                System.out.println("ERROR: queue was not deleted: rowDeleted = " + rowsDeleted);
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }
}
