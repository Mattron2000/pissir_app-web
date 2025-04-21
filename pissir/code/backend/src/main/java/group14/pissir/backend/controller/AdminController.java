package group14.pissir.backend.controller;

import java.time.LocalDate;
import java.time.LocalTime;
import java.util.ArrayList;

import group14.pissir.backend.service.AdminService;
import group14.pissir.util.schema.ErrorMessage;
import group14.pissir.util.schema.RequestData;
import group14.pissir.util.schema.ResponseMessage;
import io.javalin.http.Context;
import io.javalin.openapi.HttpMethod;
import io.javalin.openapi.OpenApi;
import io.javalin.openapi.OpenApiContent;
import io.javalin.openapi.OpenApiParam;
import io.javalin.openapi.OpenApiRequestBody;
import io.javalin.openapi.OpenApiResponse;

public class AdminController {

    private static AdminController instance = null;

    public static AdminController getInstance() {
        if (instance == null)
            return new AdminController();
        else
            return instance;
    }

    public AdminController() {
        instance = this;
    }

    // #region
    @OpenApi(
        path = "/admin/prices/hours",
        methods = HttpMethod.PUT,
        versions = { "1.0.0" },
        tags = { "Admin" },
        summary = "Set parking price",
        description = "Set parking price in DB",
        requestBody = @OpenApiRequestBody(
            description = "Float number",
            content = {
                @OpenApiContent(from = Float.class)
            }
        ),
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "Price set successfully",
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
            )
        }
    )
    @OpenApi(
        path = "/admin/prices/kw",
        methods = HttpMethod.PUT,
        versions = { "1.0.0" },
        tags = { "Admin" },
        summary = "Set charging price",
        description = "Set charging price in DB",
        requestBody = @OpenApiRequestBody(
            description = "Float number",
            content = {
                @OpenApiContent(from = Float.class)
            }
        ),
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "Price set successfully",
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
            )
        }
    )
    // #endregion
    public void setPrice(Context ctx, String type) {
        if (ctx.body() == null) {
            ctx.status(400).json(new ErrorMessage("Request body is null"));
            return;
        }

        Float price = null;

        try {
            price = Float.parseFloat(ctx.body());
        } catch (NumberFormatException e) {
            ctx.status(400).json(new ErrorMessage("Invalid body provided"));
            return;
        }

        if (price == null || price < 0) {
            ctx.status(400).json(new ErrorMessage("Invalid body provided"));
            return;
        }

        if (AdminService.getInstance().setPrice(price.floatValue(), type))
            ctx.status(200).json(new ResponseMessage("Price set successfully"));
        else
            ctx.status(400).json(new ErrorMessage("Price not set"));
    }

    // #region
    @OpenApi(
        path = "/admin/history/date",
        methods = HttpMethod.GET,
        versions = { "1.0.0" },
        tags = { "Admin" },
        summary = "Get request history by date range",
        description = "Get request history by date range in DB",
        queryParams = {
            @OpenApiParam(
                name = "date_start",
                description = "Start date",
                required = true,
                allowEmptyValue = false,
                deprecated = false,
                type = LocalDate.class,
                example = "YYYY-MM-DD"
            ),
            @OpenApiParam(
                name = "date_end",
                description = "End date",
                required = true,
                allowEmptyValue = false,
                deprecated = false,
                type = LocalDate.class,
                example = "YYYY-MM-DD"
            )
        },
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "History list",
                content = @OpenApiContent(from = RequestData[].class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Request query is null",
                content = @OpenApiContent(from = ErrorMessage.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Invalid query provided",
                content = @OpenApiContent(from = ErrorMessage.class)
            )
        }
    )
    // #endregion
    public void getRequestHistoryByData(Context ctx) {
        // REQUEST http://localhost:9999/api/v1/admin/history/date?date_start=YYYY-MM-DD&date_end=YYYY-MM-DD

        LocalDate date_start = null;
        LocalDate date_end = null;

        try {
            date_start = LocalDate.parse(ctx.queryParam("date_start"));
            date_end = LocalDate.parse(ctx.queryParam("date_end"));
        } catch (Exception e) {
            ctx.status(400).json(new ErrorMessage("Invalid date format"));
            return;
        }

        if (date_start == null || date_end == null) {
            ctx.status(400).json(new ErrorMessage("Request query is null"));
            return;
        }

        if (date_start.isAfter(date_end)) {
            ctx.status(400).json(new ErrorMessage("Invalid query provided"));
            return;
        }

        ArrayList<RequestData> history = AdminService.getInstance().getRequestHistoryByData(date_start, date_end);

        if (history == null) {
            ctx.status(404).json(new ErrorMessage("History not found"));
            return;
        }

        ctx.status(200).json(history);
    }

    // #region
    @OpenApi(
        path = "/admin/history/time",
        methods = HttpMethod.GET,
        versions = { "1.0.0" },
        tags = { "Admin" },
        summary = "Get request history by time range",
        description = "Get request history by time range in DB",
        queryParams = {
            @OpenApiParam(
                name = "time_start",
                description = "Start time",
                required = true,
                allowEmptyValue = false,
                deprecated = false,
                type = String.class,
                example = "HH:MM:SS"
            ),
            @OpenApiParam(
                name = "time_end",
                description = "End time",
                required = true,
                allowEmptyValue = false,
                deprecated = false,
                type = String.class,
                example = "HH:MM:SS"
            )
        },
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "History list",
                content = @OpenApiContent(from = RequestData[].class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Request query is null",
                content = @OpenApiContent(from = ErrorMessage.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Invalid query provided",
                content = @OpenApiContent(from = ErrorMessage.class)
            )
        }
    )
    // #endregion
    public void getRequestHistoryByTime(Context ctx) {
        // REQUEST http://localhost:9999/api/v1/admin/history/time?time_start=HH:MM:SS&time_end=HH:MM:SS

        LocalTime time_start = null;
        LocalTime time_end = null;

        try {
            time_start = LocalTime.parse(ctx.queryParam("time_start"));
            time_end = LocalTime.parse(ctx.queryParam("time_end"));
        } catch (Exception e) {
            ctx.status(400).json(new ErrorMessage("Invalid time format"));
            return;
        }

        if (time_start == null || time_end == null) {
            ctx.status(400).json(new ErrorMessage("Request query is null"));
            return;
        }

        if (time_start.isAfter(time_end)) {
            ctx.status(400).json(new ErrorMessage("Invalid query provided"));
            return;
        }

        ArrayList<RequestData> history = AdminService.getInstance().getRequestHistoryByTime(time_start, time_end);

        if (history == null) {
            ctx.status(404).json(new ErrorMessage("History not found"));
            return;
        }

        ctx.status(200).json(history);
    }

    // #region
    @OpenApi(
        path = "/admin/history",
        methods = HttpMethod.GET,
        versions = { "1.0.0" },
        tags = { "Admin" },
        summary = "Get request history",
        description = "Get request history in DB",
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "History list",
                content = @OpenApiContent(from = RequestData[].class)
            )
        }
    )
    // #endregion
    public void getRequestHistory(Context ctx) {
        ctx.status(200).json(AdminService.getInstance().getRequestHistory());
    }

    // #region
    @OpenApi(
        path = "/admin/history/type/{type}",
        pathParams = @OpenApiParam(
            name = "type",
            type = String.class,
            description = "PARKING|CHARGING",
            required = true,
            allowEmptyValue = false,
            deprecated = false,
            example = "PARKING"
        ),
        methods = HttpMethod.GET,
        versions = { "1.0.0" },
        tags = { "Admin" },
        summary = "Get request history by type",
        description = "Get request history by type in DB",
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "History list",
                content = @OpenApiContent(from = RequestData[].class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Request path is null",
                content = @OpenApiContent(from = ErrorMessage.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Invalid type provided",
                content = @OpenApiContent(from = ErrorMessage.class)
            )
        }
    )
    // #endregion
    public void getRequestHistoryByType(Context ctx) {
        // REQUEST http://localhost:9999/api/v1/admin/history/type/{PARKING|CHARGING}
        String type = null;

        try {
            type = ctx.pathParam("type");
        } catch (Exception e) {
            ctx.status(400).json(new ErrorMessage("Invalid type provided"));
            return;
        }

        if (type == null) {
            ctx.status(400).json(new ErrorMessage("Request path is null"));
            return;
        }

        if (!type.equals("PARKING") && !type.equals("CHARGING")) {
            ctx.status(400).json(new ErrorMessage("Invalid type provided"));
            return;
        }

        ArrayList<RequestData> history = AdminService.getInstance().getRequestHistoryByType(type);

        if (history == null) {
            ctx.status(404).json(new ErrorMessage("History not found"));
            return;
        }

        ctx.status(200).json(history);
    }

    // #region
    @OpenApi(
        path = "/admin/history/grade/{grade}",
        pathParams = @OpenApiParam(
            name = "grade",
            type = String.class,
            description = "BASE|PREMIUM",
            allowEmptyValue = false,
            required = true,
            example = "BASE"
        ),
        methods = HttpMethod.GET,
        versions = { "1.0.0" },
        tags = { "Admin" },
        summary = "Get request history by user grade",
        description = "Get request history by grade in DB",
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "History list",
                content = @OpenApiContent(from = RequestData[].class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Request path is null",
                content = @OpenApiContent(from = ErrorMessage.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Invalid grade provided",
                content = @OpenApiContent(from = ErrorMessage.class)
            )
        }
    )
    // #endregion
    public void getRequestHistoryByGrade(Context ctx) {
        // REQUEST http://localhost:9999/api/v1/admin/history/grade/{BASE|PREMIUM}
        String grade = null;

        try {
            grade = ctx.pathParam("grade");
        } catch (Exception e) {
            ctx.status(400).json(new ErrorMessage("Invalid grade provided"));
            return;
        }

        if (grade == null) {
            ctx.status(400).json(new ErrorMessage("Request path is null"));
            return;
        }

        if (!grade.equals("BASE") && !grade.equals("PREMIUM")) {
            ctx.status(400).json(new ErrorMessage("Invalid grade provided"));
            return;
        }

        ArrayList<RequestData> history = AdminService.getInstance().getRequestHistoryByGrade(grade);

        if (history == null) {
            ctx.status(404).json(new ErrorMessage("History not found"));
            return;
        }

        ctx.status(200).json(history);
    }
}
