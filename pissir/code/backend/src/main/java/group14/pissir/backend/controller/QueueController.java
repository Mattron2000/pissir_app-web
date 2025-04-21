package group14.pissir.backend.controller;

import group14.pissir.backend.service.QueueService;
import io.javalin.http.Context;
import io.javalin.openapi.HttpMethod;
import io.javalin.openapi.OpenApi;
import io.javalin.openapi.OpenApiContent;
import io.javalin.openapi.OpenApiResponse;

public class QueueController {

    private static QueueController instance = null;

    public static QueueController getInstance() {
        if (instance == null)
            return new QueueController();
        else
            return instance;
    }

    public QueueController() {
        instance = this;
    }

    //#region
    @OpenApi(
        path = "/queue/size",
        methods = HttpMethod.GET,
        versions = { "1.0.0" },
        tags = { "Queue" },
        summary = "Get queue size",
        description = "Returns the size of the queue of mwbot",
        responses =
            @OpenApiResponse(
                status = "200",
                description = "Queue size",
                content = @OpenApiContent(from = Integer.class)
            )
    )
    //#endregion
    public void getQueueSize(Context ctx) {
        ctx.status(200).json(QueueService.getInstance().getQueueSize());
    }

    public void deleteQueue(int slotId) {
        QueueService.getInstance().deleteQueue(slotId);
    }
}
