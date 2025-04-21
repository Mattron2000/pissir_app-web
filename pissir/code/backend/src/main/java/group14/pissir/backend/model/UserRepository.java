package group14.pissir.backend.model;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import group14.pissir.backend.db.DBConnect;
import group14.pissir.util.UserType;
import group14.pissir.util.schema.UserData;

public class UserRepository {

    private static UserRepository instance = null;

    private UserRepository() {
        instance = this;
    }

    public static UserRepository getInstance() {
        if (instance == null)
            return new UserRepository();
        else
            return instance;
    }

    public Boolean createUser(String email, String password) {
        final String sql = "INSERT INTO users (email, password, type) VALUES (?, ?, ?)";

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, email);
            st.setString(2, password);
            st.setString(3, UserType.BASE.toString());

            int rowsInserted = st.executeUpdate();

            st.close();
            conn.close();

            if (rowsInserted == 1)
                return true;
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return false;
    }

    public UserData getUser(String email) {
        final String sql = "SELECT * FROM users WHERE email = ?";
        UserData userData = null;

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, email);

            ResultSet resultQuery = st.executeQuery();

            if (resultQuery.next())
                userData = new UserData(resultQuery.getString("email"), resultQuery.getString("password"), resultQuery.getString("type"));

            if (resultQuery.next()) {
                System.out.println("More than one user found");
                return null;
            }

            st.close();
            conn.close();

            return userData;
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return null;
    }

    public boolean setType(String email, UserType type) {
        final String sql = "UPDATE users SET type = ? WHERE email = ?";

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, type.toString());
            st.setString(2, email);

            int rowsUpdated = st.executeUpdate();

            if (rowsUpdated == 1)
                return true;

            st.close();
            conn.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return false;
    }

    public UserData login(String email, String password) {
        final String sql = "SELECT * FROM users WHERE email = ? AND password = ?";

        try {
            Connection conn = DBConnect.getInstance().getConnection();
            PreparedStatement st = conn.prepareStatement(sql);

            st.setString(1, email);
            st.setString(2, password);

            ResultSet resultQuery = st.executeQuery();

            UserData userData = null;

            if (resultQuery.next())
                userData = new UserData(resultQuery.getString("email"), resultQuery.getString("password"), resultQuery.getString("type"));

            st.close();
            conn.close();

            return userData;
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return null;
    }
}
