package group14.pissir.backend.controller;

import java.time.LocalDateTime;

import group14.pissir.backend.service.SlotService;
import group14.pissir.util.JsonWrapper;
import group14.pissir.util.SensorStatus;
import group14.pissir.util.schema.ErrorMessage;
import group14.pissir.util.schema.SensorData;
import io.javalin.http.Context;
import io.javalin.openapi.HttpMethod;
import io.javalin.openapi.OpenApi;
import io.javalin.openapi.OpenApiContent;
import io.javalin.openapi.OpenApiParam;
import io.javalin.openapi.OpenApiResponse;

public class SlotController {

    private static SlotController instance = null;

    public static SlotController getInstance() {
        if (instance == null)
            return new SlotController();
        else
            return instance;
    }

    public SlotController() {
        instance = this;
    }

    // #region
    @OpenApi(
        path = "/slots/firstfreeslot",
        methods = HttpMethod.GET,
        versions = { "1.0.0" },
        tags = { "Slot" },
        summary = "Get free slot",
        description = "Returns the first free slot",
        queryParams = {
            @OpenApiParam(
                name = "datetime_start",
                type = String.class,
                description = "Start datetime",
                required = true
            ),
            @OpenApiParam(
                name = "datetime_end",
                type = String.class,
                description = "End datetime",
                required = true
            )
        },
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "First free slot",
                content = @OpenApiContent(from = Integer.class)
            ),
            @OpenApiResponse(
                status = "400",
                description = "Invalid datetime format"
            ),
            @OpenApiResponse(
                status = "404",
                description = "No free slots"
            )
        }
    )
    // #endregion
    public void getFreeSlots(Context ctx) {
        LocalDateTime datetime_start = null;
        LocalDateTime datetime_end = null;

        try {
            datetime_start = LocalDateTime.parse(ctx.queryParam("datetime_start"));
            datetime_end = LocalDateTime.parse(ctx.queryParam   ("datetime_end"));
        } catch (Exception e) {
            ctx.status(400).json(new ErrorMessage("Invalid datetime query format"));
            return;
        }

        if (datetime_start == null || datetime_end == null || datetime_start.isAfter(datetime_end)) {
            ctx.status(400).json(new ErrorMessage("Invalid datetime format"));
            return;
        }

        Integer slot_id = SlotService.getInstance().getFirstFreeSlot(datetime_start, datetime_end);

        if (slot_id == null)
            ctx.status(404).json(new ErrorMessage("No free slots"));
        else
            ctx.status(200).json(slot_id);
    }

    // #region
    @OpenApi(
        path = "/slots",
        methods = HttpMethod.GET,
        versions = { "1.0.0" },
        tags = { "Slot" },
        summary = "Get all slots",
        description = "Returns all slots",
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "List of slots",
                content = @OpenApiContent(from = SensorData[].class)
            )
        }
    )
    // #endregion
    public void getSlots(Context ctx) {
        ctx.status(200).json(SlotService.getInstance().getSlots());
    }

    public void setSlot(String message) {
        SensorData sensorData = JsonWrapper.fromJson(message, SensorData.class);

        if (sensorData == null)
            return;

        if (sensorData.slot_id() == null || sensorData.status() == null)
            return;

        if (sensorData.slot_id() <= 0)
            return;

        if (sensorData.status() != SensorStatus.FREE && sensorData.status() != SensorStatus.OCCUPIED)
            return;

        SlotService.getInstance().setSlot(sensorData.slot_id(), sensorData.status());
    }
}
