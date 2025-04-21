package group14.pissir.backend.model;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.time.LocalDateTime;
import java.util.ArrayList;

import group14.pissir.backend.db.DBConnect;
import group14.pissir.util.schema.RequestData;
import group14.pissir.util.schema.RequestTimeData;

public class RequestRepository {

    private static RequestRepository instance = null;

    private RequestRepository() {
        instance = this;
    }

    public static RequestRepository getInstance() {
        if (instance == null)
            return new RequestRepository();
        else
            return instance;
    }

    public ArrayList<RequestTimeData> getWaitingTime() {
        final String sql = "SELECT datetime(start) AS start, datetime(end) AS end FROM requests WHERE datetime(start) >= datetime('now', 'localtime') AND type = 'CHARGING'";
        ArrayList<RequestTimeData> requestTimeData = new ArrayList<>();

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            ResultSet resultQuery = st.executeQuery();

            while (resultQuery.next()) {
                LocalDateTime start = resultQuery.getTimestamp("start").toLocalDateTime();
                LocalDateTime end = resultQuery.getTimestamp("end").toLocalDateTime();
                requestTimeData.add(new RequestTimeData(start.toString(), end.toString()));
            }

            st.close();
            conn.close();

            return requestTimeData;
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return requestTimeData;
    }

    public void updateRequestKW(Integer slotId, Integer kw) {
        final String sql = """
                UPDATE requests
                SET kw = ?
                WHERE slot_id = ?
                AND datetime_start = (
                    SELECT datetime_start
                    FROM requests
                    WHERE slot_id = ? AND kw IS NULL
                    ORDER BY datetime_start DESC
                    LIMIT 1
                )
                """;

        // "UPDATE requests SET kw = ? WHERE slot_id = ? ORDER BY datetime_start DESC LIMIT 1";

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setInt(1, kw);
            st.setInt(2, slotId);
            st.setInt(3, slotId);

            int rowsUpdated = st.executeUpdate();

            st.close();
            conn.close();

            if (rowsUpdated != 1)
                System.out.println("ERROR: request was not updated: rowUpdated = " + rowsUpdated);
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    public RequestData getRequestPaid(String email, LocalDateTime datetime_start) {
        final String sql = """
                SELECT email, type, datetime_start, datetime_end, kw, slot_id
                FROM requests JOIN users USING (email)
                WHERE email = ? AND datetime(datetime_start) = datetime(?)
                """;

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, email);
            st.setTimestamp(2, java.sql.Timestamp.valueOf(datetime_start));

            ResultSet resultQuery = st.executeQuery();

            if (resultQuery.next()) {
                String email2 = resultQuery.getString("email");
                String type = resultQuery.getString("type");
                String datetime_start2 = resultQuery.getString("datetime_start");
                String datetime_end = resultQuery.getString("datetime_end");
                Integer kw = resultQuery.getInt("kw");
                if (resultQuery.wasNull()) kw = null;
                int slot_id = resultQuery.getInt("slot_id");

                st.close();
                conn.close();

                return new RequestData(email2, type, datetime_start2, datetime_end, kw, slot_id);
            }

            st.close();
            conn.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return null;
    }

    public boolean updateRequest(String email) {
        final String sql = """
                UPDATE requests
                SET paid = true
                WHERE email = ? AND paid = false
                """;

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, email);

            int rowsUpdated = st.executeUpdate();

            st.close();
            conn.close();

            if (rowsUpdated == 1)
                return true;

            return false;
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return false;
    }

    public RequestData getUserUnpaidRequest(String email) {
        final String sql = """
                SELECT email, type, datetime_start, datetime_end, kw, slot_id
                FROM requests JOIN users USING (email)
                WHERE email = ? AND paid = false
                """;

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, email);

            ResultSet resultQuery = st.executeQuery();

            if (resultQuery.next()) {
                String email2 = resultQuery.getString("email");
                String type = resultQuery.getString("type");
                String datetime_start = resultQuery.getString("datetime_start");
                String datetime_end = resultQuery.getString("datetime_end");
                Integer kw = resultQuery.getInt("kw");
                if (resultQuery.wasNull()) kw = null;
                int slot_id = resultQuery.getInt("slot_id");

                st.close();
                conn.close();

                return new RequestData(email2, type, datetime_start, datetime_end, kw, slot_id);
            }

            st.close();
            conn.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return null;
    }

    public boolean addRequest(String email, String datetime_start, String datetime_end, Integer kw, boolean paid, int slot_id) {
        final String sql = """
                INSERT INTO requests (email, datetime_start, datetime_end, slot_id)
                VALUES (?, datetime(?), datetime(?), ?)
                """;

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, email);
            st.setString(2, datetime_start);
            st.setString(3, datetime_end);
            st.setInt(4, slot_id);

            int rowsInserted = st.executeUpdate();

            st.close();
            conn.close();

            if (rowsInserted == 1)
                return true;

            return false;
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return false;
    }

    public ArrayList<RequestData> getUserUnpaidRequests(String email) {
        final String sql = """
                SELECT email, type, datetime_start, datetime_end, kw, slot_id
                FROM requests JOIN users USING (email)
                WHERE email = ? AND paid = false
                """;

        ArrayList<RequestData> requests = new ArrayList<>();

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, email);

            ResultSet resultQuery = st.executeQuery();

            while (resultQuery.next()) {
                String email2 = resultQuery.getString("email");
                String type = resultQuery.getString("type");
                String datetime_start = resultQuery.getString("datetime_start");
                String datetime_end = resultQuery.getString("datetime_end");
                Integer kw = resultQuery.getInt("kw");
                if (resultQuery.wasNull()) kw = null;
                int slot_id = resultQuery.getInt("slot_id");

                requests.add(new RequestData(email2, type, datetime_start, datetime_end, kw, slot_id));
            }

            st.close();
            conn.close();

            return requests;
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return requests;
    }
}
