package group14.pissir.backend.db;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

public class DBConnect {

    // the DB location (on file, inside the project)
    static private final String dbLoc = "jdbc:sqlite:src/main/resources/smartparking.db";

    static private DBConnect instance = null;

    private DBConnect() {
        instance = this;
    }

    public static DBConnect getInstance() {
        if (instance == null)
            return new DBConnect();
        else
            return instance;
    }

    public Connection getConnection() throws SQLException {
        try {
            // return the connection instance
            return DriverManager.getConnection(dbLoc);
        } catch (SQLException e) {
            throw new SQLException("Cannot get connection to " + dbLoc, e);
        }
    }
}
