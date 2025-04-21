package group14.pissir.backend.model;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.util.ArrayList;

import group14.pissir.backend.db.DBConnect;
import group14.pissir.util.schema.EmailDatetimesData;
import group14.pissir.util.schema.ReservationDatetimesSlotData;

public class ReservationRepository {

    private static ReservationRepository instance = null;

    public static ReservationRepository getInstance() {
        if (instance == null)
            return new ReservationRepository();
        else
            return instance;
    }

    public ReservationRepository() {
        instance = this;
    }

    public ArrayList<ReservationDatetimesSlotData> getUserReservations(String email) {
        final String sql = """
            SELECT datetime_start, datetime_end, slot_id
            FROM reservations
            WHERE email = ?
        """;

        ArrayList<ReservationDatetimesSlotData> result = new ArrayList<ReservationDatetimesSlotData>();

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, email);

            ResultSet resultQuery = st.executeQuery();

            while (resultQuery.next())
                result.add(new ReservationDatetimesSlotData(resultQuery.getString("datetime_start"), resultQuery.getString("datetime_end"), resultQuery.getInt("slot_id")));

            st.close();
            conn.close();

            return result;
        } catch (Exception e) {
            e.printStackTrace();
        }

        return null;
    }

    public boolean addReservation(String email, int slot_id, String datetime_start, String datetime_end) {
        final String sql = """
                INSERT INTO reservations (email, slot_id, datetime_start, datetime_end)
                VALUES (?, ?, ?, ?)
                """;

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, email);
            st.setInt(2, slot_id);
            st.setString(3, datetime_start);
            st.setString(4, datetime_end);

            int rowsInserted = st.executeUpdate();

            st.close();
            conn.close();

            if (rowsInserted == 1)
                return true;

            return false;
        } catch (Exception e) {
            e.printStackTrace();
        }

        return false;
    }

    public boolean deleteReservation(String email, String datetime_start) {
        final String sql = """
                DELETE FROM reservations
                WHERE email = ? AND datetime_start = ?
                """;

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, email);
            st.setString(2, datetime_start);

            int rowsDeleted = st.executeUpdate();

            st.close();
            conn.close();

            if (rowsDeleted == 1)
                return true;

            return false;
        } catch (Exception e) {
            e.printStackTrace();
        }

        return false;
    }

    public ArrayList<EmailDatetimesData> getReservations() {
        final String sql = """
            SELECT email, datetime_start, datetime_end
            FROM reservations
        """;

        ArrayList<EmailDatetimesData> result = new ArrayList<EmailDatetimesData>();

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            ResultSet resultQuery = st.executeQuery();

            while (resultQuery.next())
                result.add(new EmailDatetimesData(resultQuery.getString("email"), resultQuery.getString("datetime_start"), resultQuery.getString("datetime_end")));

            st.close();
            conn.close();

            return result;
        } catch (Exception e) {
            e.printStackTrace();
        }

        return result;
    }
}
