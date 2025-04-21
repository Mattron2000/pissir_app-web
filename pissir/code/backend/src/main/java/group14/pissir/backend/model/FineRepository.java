package group14.pissir.backend.model;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.time.LocalDateTime;
import java.util.ArrayList;

import group14.pissir.backend.db.DBConnect;
import group14.pissir.util.schema.DatetimesPaidData;
import group14.pissir.util.schema.EmailDatetimesPaidData;

public class FineRepository {

    private static FineRepository instance = null;

    private FineRepository() {
        instance = this;
    }

    public static FineRepository getInstance() {
        if (instance == null)
            return new FineRepository();
        else
            return instance;
    }

    public boolean addFine(String email, LocalDateTime datetime_start, LocalDateTime datetime_end) {
        return addFine(email, datetime_start.toString(), datetime_end.toString());
    }

    public boolean addFine(String email, String datetime_start, String datetime_end) {
        final String sql = """
                INSERT INTO fines (email, datetime_start, datetime_end)
                VALUES (?, ?, ?)
            """;

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, email);
            st.setString(2, datetime_start);
            st.setString(3, datetime_end);

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

    public ArrayList<EmailDatetimesPaidData> getFines() {
        final String sql = """
                SELECT *
                FROM fines
            """;

        ArrayList<EmailDatetimesPaidData> fines = new ArrayList<>();

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            ResultSet resultQuery = st.executeQuery();

            while (resultQuery.next())
                fines.add(new EmailDatetimesPaidData(resultQuery.getString("email"), resultQuery.getString("datetime_start"), resultQuery.getString("datetime_end"), resultQuery.getBoolean("paid")));

            st.close();
            conn.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return fines;
    }

    public ArrayList<DatetimesPaidData> getUserFines(String email) {
        final String sql = """
                SELECT datetime_start, datetime_end
                FROM fines
                WHERE email = ? AND paid = false
            """;

        ArrayList<DatetimesPaidData> fines = new ArrayList<>();

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, email);

            ResultSet resultQuery = st.executeQuery();

            while (resultQuery.next())
                fines.add(new DatetimesPaidData(resultQuery.getString("datetime_start"), resultQuery.getString("datetime_end"), false));

            st.close();
            conn.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return fines;
    }

    public boolean updateFine(String email, String localDateTime) {
        final String sql = """
                UPDATE fines
                SET paid = true
                WHERE email = ? AND datetime_start = ?
            """;

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, email);
            st.setString(2, localDateTime);

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
}
