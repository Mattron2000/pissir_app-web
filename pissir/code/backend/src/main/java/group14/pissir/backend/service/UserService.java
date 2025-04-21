package group14.pissir.backend.service;

import group14.pissir.backend.model.UserRepository;
import group14.pissir.util.UserType;
import group14.pissir.util.schema.UserData;

public class UserService {

    private static UserService instance = null;

    private UserService() {
        instance = this;
    }

    public static UserService getInstance() {
        if (instance == null)
            return new UserService();
        else
            return instance;
    }

    /**
     * Creates a new user in the database.
     *
     * @param email the email of the new user
     * @param password the password of the new user
     * @return true if the user was successfully created, false otherwise
     */
    public Boolean addUser(String email, String password) {
        return UserRepository.getInstance().createUser(email, password);
    }

    /**
     * Gets user data from the database based on the provided email and password.
     *
     * @param email the email of the user
     * @param password the password of the user
     * @return the user data if the user exists, null otherwise
     */
    public UserData getUser(String email) {
        return UserRepository.getInstance().getUser(email);
    }

    /**
     * Sets the type of a user in the database based on the provided email and password.
     *
     * @param email the email of the user
     * @param password the password of the user
     * @param type the new type for the user
     * @return true if the user's type was successfully updated, false otherwise
     */
    public Boolean setType(String email, UserType type) {
        return UserRepository.getInstance().setType(email, type);
    }

    public UserData login(String email, String password) {
        return UserRepository.getInstance().login(email, password);
    }
}
