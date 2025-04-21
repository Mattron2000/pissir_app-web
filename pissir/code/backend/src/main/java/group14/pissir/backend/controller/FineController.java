package group14.pissir.backend.controller;

import java.time.LocalDateTime;

import group14.pissir.backend.service.FineService;
import group14.pissir.util.schema.EmailDatetimeStartData;
import group14.pissir.util.schema.EmailDatetimesData;
import group14.pissir.util.schema.EmailDatetimesPaidData;
import group14.pissir.util.schema.ErrorMessage;
import group14.pissir.util.schema.ResponseMessage;
import io.javalin.http.Context;
import io.javalin.openapi.HttpMethod;
import io.javalin.openapi.OpenApi;
import io.javalin.openapi.OpenApiContent;
import io.javalin.openapi.OpenApiParam;
import io.javalin.openapi.OpenApiRequestBody;
import io.javalin.openapi.OpenApiResponse;

public class FineController {

    private static FineController instance = null;

    private FineController() {
        instance = this;
    }

    public static FineController getInstance() {
        if (instance == null)
            return new FineController();
        else
            return instance;
    }

    //#region
    @OpenApi(
        path = "/fines",
        methods = HttpMethod.GET,
        versions = { "1.0.0" },
        tags = { "Fine" },
        summary = "Get all fines",
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "Fines retrieved",
                content = @OpenApiContent(
                    from = EmailDatetimesPaidData[].class
                )
            )
        }
    )
    //#endregion
    public void getFines(Context ctx) {
        ctx.status(200).json(FineService.getInstance().getFines());
    }

    //#region
    @OpenApi(
        path = "/fines/{email}",
        pathParams = @OpenApiParam(
            name = "email",
            type = String.class,
            description = "User email",
            required = true,
            allowEmptyValue = false
        ),
        methods = HttpMethod.GET,
        versions = { "1.0.0" },
        tags = { "Fine" },
        summary = "Get user fines",
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "Fines retrieved",
                content = @OpenApiContent(
                    from = EmailDatetimesPaidData[].class
                )
            )
        }
    )
    //#endregion
    public void getUserFines(Context ctx) {
        String email = null;

        try {
            email = ctx.pathParam("email");
        } catch (Exception e) {
            ctx.status(400);
            ctx.result("Bad request");
            return;
        }

        ctx.status(200).json(FineService.getInstance().getUserFines(email));
    }

    //#region
    @OpenApi(
        path = "/fines",
        methods = HttpMethod.POST,
        versions = { "1.0.0" },
        tags = { "Fine" },
        summary = "Add fine",
        description = "Add fine to DB",
        requestBody = @OpenApiRequestBody(
            description = "json containing email, datetime_start and datetime_end",
            content = {
                @OpenApiContent(from = EmailDatetimesData.class)
            }
        ),
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "Fine added successfully",
                content = @OpenApiContent(from = ResponseMessage.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Request body is null",
                content = @OpenApiContent(from = ErrorMessage.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Invalid body provided",
                content = @OpenApiContent(from = ErrorMessage.class)
            ),
            @OpenApiResponse(
                status = "409",
                description = "Fine already exists",
                content = @OpenApiContent(from = ErrorMessage.class)
            )
        }
    )
    //#endregion
    public void addFine(Context ctx) {
        EmailDatetimesData body = null;

        try {
            body = ctx.bodyAsClass(EmailDatetimesData.class);
        } catch (Exception e) {
            ctx.status(400);
            ctx.result("Bad request");
            return;
        }

        if (body == null) {
            ctx.status(400);
            ctx.result("Bad request");
            return;
        }

        if (FineService.getInstance().addFine(body.email(), LocalDateTime.parse(body.datetime_start()), LocalDateTime.parse(body.datetime_end()))) {
            ctx.status(200).json(new ResponseMessage("Fine added"));
        } else {
            ctx.status(409).json(new ErrorMessage("Fine already exists"));
        }
    }

    //#region
    @OpenApi(
        path = "/fines",
        methods = HttpMethod.PUT,
        versions = { "1.0.0" },
        tags = { "Fine" },
        summary = "Update fine",
        description = "Update fine in DB",
        requestBody = @OpenApiRequestBody(
            description = "json containing email and datetime_start",
            content = {
                @OpenApiContent(from = EmailDatetimeStartData.class)
            }
        ),
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "Fine updated successfully",
                content = @OpenApiContent(from = ResponseMessage.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Request body is null",
                content = @OpenApiContent(from = ErrorMessage.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Invalid body provided",
                content = @OpenApiContent(from = ErrorMessage.class)
            ),
            @OpenApiResponse(
                status = "404",
                description = "Fine not found",
                content = @OpenApiContent(from = ErrorMessage.class)
            )
        }
    )
    //#endregion
    public void updateFine(Context ctx) {
        EmailDatetimeStartData body = null;

        try {
            body = ctx.bodyAsClass(EmailDatetimeStartData.class);
        } catch (Exception e) {
            ctx.status(400);
            ctx.result("Bad request");
            return;
        }

        if (body == null) {
            ctx.status(400);
            ctx.result("Bad request");
            return;
        }

        if (FineService.getInstance().updateFine(body.email(), body.datetime_start())) {
            ctx.status(200).json(new ResponseMessage("Fine updated"));
        } else {
            ctx.status(404).json(new ErrorMessage("Fine not found"));
        }
    }
}
