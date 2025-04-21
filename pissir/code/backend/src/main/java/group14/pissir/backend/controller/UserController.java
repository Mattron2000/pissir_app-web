package group14.pissir.backend.controller;

import java.util.HashMap;

import group14.pissir.backend.service.UserService;
import group14.pissir.util.UserType;
import group14.pissir.util.schema.ErrorMessage;
import group14.pissir.util.schema.NewUserRequest;
import group14.pissir.util.schema.ResponseMessage;
import group14.pissir.util.schema.UserData;
import group14.pissir.util.schema.UserEmailTypeData;
import group14.pissir.util.schema.UserLogin;
import group14.pissir.util.schema.UserTypeData;
import io.javalin.http.BadRequestResponse;
import io.javalin.http.Context;
import io.javalin.openapi.HttpMethod;
import io.javalin.openapi.OpenApi;
import io.javalin.openapi.OpenApiContent;
import io.javalin.openapi.OpenApiParam;
import io.javalin.openapi.OpenApiRequestBody;
import io.javalin.openapi.OpenApiResponse;

public class UserController {

    private static UserController instance = null;

    private final static String emailRegex = "^[a-zA-Z.0-9-]+@[a-zA-Z.-]+.[a-zA-Z]{2,4}$";

    private UserController() {
        instance = this;
    }

    public static UserController getInstance() {
        if (instance == null)
            return new UserController();
        else
            return instance;
    }

    // #region
    @OpenApi(
        path = "/register",
        methods = HttpMethod.POST,
        versions = { "1.0.0" },
        tags = { "User" },
        summary = "Create a user",
        description = "Insert a new user in DB",
        requestBody = @OpenApiRequestBody(
            description = "User object",
            content = {
                @OpenApiContent(from = NewUserRequest.class)
            }
        ),
        responses = {
            @OpenApiResponse(
                status = "201",
                description = "User created successfully",
                content = @OpenApiContent(from = ResponseMessage.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Request body is null",
                content = @OpenApiContent(from = ErrorMessage.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Invalid email or password provided",
                content = @OpenApiContent(from = ErrorMessage.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Request body is invalid",
                content = @OpenApiContent(from = ErrorMessage.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "User creation failed",
                content = @OpenApiContent(from = ErrorMessage.class)
            ),
            @OpenApiResponse(
                status = "409",
                description = "User already exists",
                content = @OpenApiContent(from = ErrorMessage.class)
            )
        }
    )
    // #endregion
    public void registerNewUser(Context ctx) throws Exception {
        NewUserRequest request = null;

        // REQUEST BODY
        // {
        //     "email": "email",
        //     "password": "password"
        // }

        try {
            request = ctx.bodyAsClass(NewUserRequest.class);
        } catch (BadRequestResponse e) {
            ctx.status(400).result(e.getMessage());
            return;
        } catch (Exception e) {
            switch (e.getClass().getName()) {
                case "com.fasterxml.jackson.databind.exc.MismatchedInputException":
                    ctx.status(400).json(new ErrorMessage("Request body is null"));
                    return;
                default:
                    throw e;
            }
        }

        if (request.email() == null || request.password() == null || !request.email().matches(emailRegex) || request.password().length() < 8) {
            ctx.status(400).json(new ErrorMessage("Invalid email or password provided"));
            return;
        }

        // Check if user already exists
        if (UserService.getInstance().login(request.email(), request.password()) != null) {
            ctx.status(409).json(new ErrorMessage("User already exists"));
            return;
        }

        if (UserService.getInstance().addUser(request.email(), request.password()))
            ctx.status(201).json(new ResponseMessage("User created successfully"));
        else
            ctx.status(400).json(new ErrorMessage("User creation failed"));
    }

    // #region
    @OpenApi(
        path = "/login",
        methods = HttpMethod.GET,
        versions = { "1.0.0" },
        tags = { "User" },
        summary = "Check that user exists",
        description = "Returns a result of user login",
        queryParams ={
            @OpenApiParam(
                name = "email",
                type = String.class,
                description = "User email",
                required = true
            ),
            @OpenApiParam(
                name = "password",
                type = String.class,
                description = "User password",
                required = true
            )
        },
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "The user with this email and password exist",
                content = @OpenApiContent(from = UserEmailTypeData.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Invalid email or password provided",
                content = @OpenApiContent(from = ErrorMessage.class)
            ),
            @OpenApiResponse(
                status = "404",
                description = "No user has this email and password",
                content = @OpenApiContent(from = ErrorMessage.class)
            )
        }
    )
    // #endregion
    public void userLogin(Context ctx) {
        UserLogin user = null;

        // REQUEST
        // http://localhost:9999/api/v1/login?email=email@email.com&password=password

        try {
            user = new UserLogin(ctx.queryParam("email"), ctx.queryParam("password"));
        } catch (BadRequestResponse e) {
        } catch (Exception e) {}

        if (user.email() == null || user.password() == null || !user.email().matches(emailRegex) || user.password().length() < 8) {
            ctx.status(400).json(new ErrorMessage("Invalid email or password provided"));
            return;
        }

        UserData userData = UserService.getInstance().login(user.email(), user.password());

        if (userData == null) {
            ctx.status(404).json(new ErrorMessage("User with this email and password does not exist"));
            return;
        }

        HashMap<String, String> userHMap = new HashMap<>();

        userHMap.put("email", userData.email());
        userHMap.put("type", userData.type());

        ctx.status(200).json(userHMap);
    }

    // #region
    @OpenApi(
        path = "/users/{email}/type",
        pathParams = @OpenApiParam(
            name = "email",
            type = String.class,
            description = "User email",
            required = true
        ),
        methods = HttpMethod.PUT,
        versions = { "1.0.0" },
        tags = { "User" },
        summary = "Set user type",
        description = "Set user type in DB",
        requestBody = @OpenApiRequestBody(
            description = "User object",
            content = {
                @OpenApiContent(from = UserTypeData.class)
            }
        ),
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "User modified successfully",
                content = @OpenApiContent(from = ResponseMessage.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Request body is invalid",
                content = @OpenApiContent(from = ErrorMessage.class)
            ),
            @OpenApiResponse(
                status = "404",
                description = "User not found",
                content = @OpenApiContent(from = ErrorMessage.class)
            )
        }
    )
    // #endregion
    public void setUserType(Context ctx) throws Exception {
        HashMap<String, String> userHMap = new HashMap<>();

        // REQUEST BODY
        // {
        //     "type": "newType"
        // }

        try {
            userHMap.put("email", ctx.pathParam("email"));
            userHMap.put("type", ctx.bodyAsClass(UserTypeData.class).type());
        } catch (Exception e) {
            switch (e.getClass().getName()) {
                case "com.fasterxml.jackson.databind.exc.MismatchedInputException":
                    ctx.status(400).json(new ErrorMessage("Request body is null"));
                    return;
                default:
                    throw e;
            }
        }

        UserData user = new UserData(userHMap.get("email"),"", userHMap.get("type"));

        if (user.email() == null || !user.email().matches(emailRegex) || user.type() == null || !(user.type().equals("BASE") || user.type().equals("PREMIUM"))) {
            if (user.type().equals("ADMIN")) {
                ctx.status(403).json(new ErrorMessage("Cannot have admin privileges"));
                return;
            }
            ctx.status(400).json(new ErrorMessage("Invalid email provided"));
            return;
        }

        // Check if user exists
        UserData userData = UserService.getInstance().getUser(user.email());

        if (userData == null) {
            ctx.status(404).json(new ErrorMessage("User with this email not found"));
            return;
        }

        if (userData.type().equals(UserType.ADMIN.toString())) {
            ctx.status(403).json(new ErrorMessage("Admin user cannot be modified"));
            return;
        }

        if (userData.type().equals(user.type())) {
            ctx.status(200).json(new ResponseMessage("User already has this type"));
            return;
        }

        if (UserService.getInstance().setType(user.email(), UserType.valueOf(user.type())))
            ctx.status(200).json(new ResponseMessage("User modified successfully"));
        else
            ctx.status(404).json(new ErrorMessage("User with this email and password not found"));
    }
}
