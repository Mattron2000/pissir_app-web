package group14.pissir.backend.controller;

import java.util.ArrayList;

import group14.pissir.backend.service.RequestService;
import group14.pissir.util.schema.ErrorMessage;
import group14.pissir.util.schema.RequestData;
import group14.pissir.util.schema.ResponseMessage;
import io.javalin.http.Context;
import io.javalin.openapi.HttpMethod;
import io.javalin.openapi.OpenApi;
import io.javalin.openapi.OpenApiContent;
import io.javalin.openapi.OpenApiParam;
import io.javalin.openapi.OpenApiResponse;

public class RequestController {

    private static RequestController instance = null;

    private RequestController() {
        instance = this;
    }

    public static RequestController getInstance() {
        if (instance == null)
            return new RequestController();
        else
            return instance;
    }

    @OpenApi(
        path = "/payments/{email}",
        pathParams = @OpenApiParam(
            name = "email",
            type = String.class,
            description = "User email",
            required = true,
            allowEmptyValue = false
        ),
        methods = HttpMethod.PUT,
        versions = { "1.0.0" },
        tags = { "Request" },
        summary = "Update request data",
        description = "",
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "Reservation data updated",
                content = @OpenApiContent(from = ResponseMessage.class)
            ),
            @OpenApiResponse(
                status = "404",
                description = "Request not found",
                content = @OpenApiContent(from = ErrorMessage.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Invalid email or datetime provided",
                content = @OpenApiContent(from = ErrorMessage.class)
            )
        }
    )
    // #endregion
    public void updatePayment(Context ctx) {

        String email = null;
        try {
            email = ctx.pathParam("email");
        } catch (Exception e) {
            ctx.status(400).json(new ErrorMessage("Invalid email provided"));
        }

        if (email == null) {
            ctx.status(400).json(new ErrorMessage("Invalid email provided"));
            return;
        }

        RequestData request = RequestService.getInstance().getUserUnpaidRequest(email);

        if (request == null) {
            ctx.status(404).json(new ErrorMessage("Request not found"));
            return;
        }

        if (RequestService.getInstance().updatePayment(request.email())) {
            ctx.status(200).json(new ResponseMessage("Request updated"));
        } else
            ctx.status(404).json(new ErrorMessage("Request not found"));

        // response: {
        //     200,
        //     404 ctx.status(404).json(new ErrorMessage("Invalid email or password provided"));
        // }
    }

    //#region
    @OpenApi(
        path = "/requests/{email}/unpaid",
        pathParams = @OpenApiParam(
            name = "email",
            type = String.class,
            description = "User email",
            required = true,
            allowEmptyValue = false
        ),
        methods = HttpMethod.GET,
        versions = { "1.0.0" },
        tags = { "Request" },
        summary = "Get user unpaid request",
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "Request retrieved",
                content = @OpenApiContent(from = RequestData[].class)
            ),
            @OpenApiResponse(
                status = "404",
                description = "Request not found",
                content = @OpenApiContent(from = ErrorMessage.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Invalid email provided",
                content = @OpenApiContent(from = ErrorMessage.class)
            )
        }
        )

    //#endregion
    public void getUnpaidUserRequest(Context ctx) {
        String email = null;
        try {
            email = ctx.pathParam("email");
        } catch (Exception e) {
            ctx.status(400).json(new ErrorMessage("Invalid email provided"));
            return;
        }

        if (email == null) {
            ctx.status(400).json(new ErrorMessage("Invalid email provided"));
            return;
        }

        ArrayList<RequestData> request = RequestService.getInstance().getUserUnpaidRequests(email);

        ctx.status(200).json(request);
    }
}
