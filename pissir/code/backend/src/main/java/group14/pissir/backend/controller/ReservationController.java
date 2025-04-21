package group14.pissir.backend.controller;

import java.util.ArrayList;

import group14.pissir.backend.Backend;
import group14.pissir.backend.service.QueueService;
import group14.pissir.backend.service.RequestService;
import group14.pissir.backend.service.ReservationService;
import group14.pissir.util.JsonWrapper;
import group14.pissir.util.MqttUtil.Sensor;
import group14.pissir.util.TOPICS;
import group14.pissir.util.schema.ErrorMessage;
import group14.pissir.util.schema.MWbotRequest;
import group14.pissir.util.schema.NewReservationData;
import group14.pissir.util.schema.ReservationDatetimesSlotData;
import group14.pissir.util.schema.ResponseMessage;
import group14.pissir.util.schema.UserReservationData;
import group14.pissir.util.schema.newMobile;
import io.javalin.http.BadRequestResponse;
import io.javalin.http.Context;
import io.javalin.openapi.HttpMethod;
import io.javalin.openapi.OpenApi;
import io.javalin.openapi.OpenApiContent;
import io.javalin.openapi.OpenApiParam;
import io.javalin.openapi.OpenApiRequestBody;
import io.javalin.openapi.OpenApiResponse;

public class ReservationController {

    private static ReservationController instance = null;

    public static ReservationController getInstance() {
        if (instance == null)
            return new ReservationController();
        else
            return instance;
    }

    public ReservationController() {
        instance = this;
    }

    // #region
    @OpenApi(
        path = "/reservations/{email}",
        pathParams = @OpenApiParam(
            name = "email",
            type = String.class,
            description = "User email",
            required = true
        ),
        methods = HttpMethod.GET,
        versions = { "1.0.0" },
        tags = { "Reservation" },
        summary = "get user reservations by email",
        description = "Returns the reservations of a user",
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "List of user reservations",
                content = @OpenApiContent(from = UserReservationData[].class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Request body is null"
            ),
            @OpenApiResponse(
                status = "400",
                description = "Request body is invalid"
            ),
            @OpenApiResponse(
                status = "404",
                description = "User not found"
            )
        }
    )
    // #endregion
    public void getUserReservations(Context ctx) {
        // REQUEST http://localhost:9999/api/v1/reservations/{email}
        String email = null;

        try {
            email = ctx.pathParam("email");
        } catch (BadRequestResponse e) {
            ctx.status(400).json(new ErrorMessage(e.getMessage()));
            return;
        } catch (Exception e) {
            switch (e.getClass().getName()) {
                case "com.fasterxml.jackson.databind.exc.MismatchedInputException":
                    ctx.status(400).result("Request body is null");
                    return;
                default:
                    throw e;
            }
        }

        ArrayList<ReservationDatetimesSlotData> reservations = new ArrayList<>();
        reservations = ReservationService.getInstance().getUserReservations(email);

        if (reservations == null) {
            ctx.status(404).json(new ErrorMessage("User not found"));
            return;
        }

        ctx.status(200).json(reservations);
    }

    // #region
    @OpenApi(
        path = "/reservations",
        methods = HttpMethod.POST,
        versions = { "1.0.0" },
        tags = { "Reservation" },
        summary = "Add user details about reservation or request",
        requestBody = @OpenApiRequestBody(
            description = "User details",
            content = {
                @OpenApiContent(from = NewReservationData.class)
            },
            required = true
        ),
        responses = {
            @OpenApiResponse(
                status = "201",
                description = "New reservation/request added",
                content = @OpenApiContent(from = ResponseMessage.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Request body is null",
                content = @OpenApiContent(from = ErrorMessage.class)
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
    public void addDetail(Context ctx) {
        // json body: prenotazione a distanza
        // {
        //     "email": "mariorossi@gmail.com",
        //     "slot_id": 1,
        //     "datetime_start": "2025-02-26 16:00",
        //     "datetime_end": "2025-02-26 16:30",
        //     "percentage": 50 | null,
        //     "reservation": true | false,
        //     "phone_number": "1234567890" | ""
        // }

        NewReservationData newDetail = null;

        try {
            newDetail = ctx.bodyAsClass(NewReservationData.class);
        } catch (BadRequestResponse e) {
            ctx.status(400).json(new ErrorMessage(e.getMessage()));
            return;
        } catch (Exception e) {
            switch (e.getClass().getName()) {
                case "com.fasterxml.jackson.databind.exc.MismatchedInputException":
                    ctx.status(400).result("Request body is null");
                    return;
                default:
                    throw e;
            }
        }

        if (newDetail == null) {
            ctx.status(400).json(new ErrorMessage("Request body is null"));
            return;
        }

        if (newDetail.reservation()) {
            addReservation(ctx, newDetail);
            return;
        }

        addRequest(ctx, newDetail);

        if (newDetail.percentage() != null && newDetail.percentage() > 0) {
            QueueService.getInstance().addQueue(newDetail.slot_id(), newDetail.percentage(), newDetail.phone_number());

            Backend.getInstance().publish(TOPICS.MWBOT.getNewRequest(), JsonWrapper.toJson(new MWbotRequest(newDetail.slot_id(), newDetail.percentage(), newDetail.phone_number())));

            if (newDetail.phone_number() != null && newDetail.phone_number().matches("^\\d{10}$"))
                Backend.getInstance().publish(TOPICS.MOBILE.getNew(newDetail.phone_number()), JsonWrapper.toJson(new newMobile(newDetail.phone_number())));
        }
    }

    private void addRequest(Context ctx, NewReservationData newDetail) {
        if (RequestService.getInstance().AddRequest(newDetail.email(),
                newDetail.datetime_start(), newDetail.datetime_end(), null, false,
                newDetail.slot_id())) {
            ctx.status(201).json(new ResponseMessage("Request added"));

            Backend.getInstance().publish(TOPICS.SENSOR.getSwitch(Sensor.idPrefix + newDetail.slot_id()), "OCCUPIED");
        } else {
            ctx.status(404).json(new ErrorMessage("User not found, slot not available or request already exists"));
        }
    }

    private void addReservation(Context ctx, NewReservationData newDetail) {
        if (ReservationService.getInstance().addReservation(newDetail.email(), newDetail.slot_id(),
                newDetail.datetime_start(), newDetail.datetime_end())) {
            ctx.status(201).json(
                    new UserReservationData(newDetail.datetime_start(), newDetail.datetime_end(), newDetail.slot_id()));

            Backend.getInstance().schedule(newDetail.email(), newDetail.datetime_start(), newDetail.datetime_end());
        } else {
            ctx.status(404).json(new ErrorMessage("User not found, slot not available or reservation already exists"));
        }
    }
}
