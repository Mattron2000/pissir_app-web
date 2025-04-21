package group14.pissir.backend;

import static io.javalin.apibuilder.ApiBuilder.before;
import static io.javalin.apibuilder.ApiBuilder.get;
import static io.javalin.apibuilder.ApiBuilder.path;
import static io.javalin.apibuilder.ApiBuilder.post;
import static io.javalin.apibuilder.ApiBuilder.put;

import java.util.function.Consumer;

import group14.pissir.backend.controller.AdminController;
import group14.pissir.backend.controller.FineController;
import group14.pissir.backend.controller.PriceController;
import group14.pissir.backend.controller.QueueController;
import group14.pissir.backend.controller.RequestController;
import group14.pissir.backend.controller.ReservationController;
import group14.pissir.backend.controller.SlotController;
import group14.pissir.backend.controller.UserController;
import io.javalin.openapi.plugin.OpenApiPlugin;
import io.javalin.openapi.plugin.swagger.SwaggerPlugin;
import io.javalin.util.JavalinLogger;

public class JavalinBackendConfig {

    public static Consumer<io.javalin.config.JavalinConfig> getConfig() {
        return config -> {
            config.registerPlugin(new OpenApiPlugin(pluginConfig -> {
                pluginConfig
                .withDefinitionConfiguration((version, definition) ->
                    definition
                        .withInfo(info ->
                            info
                                .title("SmartParking - OpenAPI 3.0")
                                .version("1.0.0")
                                .description("Questa è la pagina di documentazione della Smart Parking API")
                        ).withServer(server ->
                            server
                                .description("Smart Parking - OpenApi 3.0")
                                .url("http://localhost:{port}/api/{version}/")
                                .variable("port", "Server's port", "9999", "9999")
                                .variable("version", "Base path versioning of the server", "v1", "v1")
                        )
                );
            }));
            config.registerPlugin(new SwaggerPlugin(pluginConfig -> {
                pluginConfig.setTitle("Smart Parking API");
            }));
            config.router.apiBuilder(() -> {
                before(ctx -> {
                    JavalinLogger.info(ctx.req().getMethod() + " " + ctx.fullUrl());
                });
                path("/api", () -> {
                    path("/v1", () -> {
                        post("/register", ctx -> UserController.getInstance().registerNewUser(ctx));
                        get("/login", ctx -> UserController.getInstance().userLogin(ctx));
                        put("/users/<email>/type", ctx -> UserController.getInstance().setUserType(ctx));
                        // get("/waitingTime", ctx -> RequestController.getInstance().getWaitingTime(ctx));
                        path("/slots", () -> {
                            get("", ctx -> SlotController.getInstance().getSlots(ctx));
                            get("/firstfreeslot", ctx -> SlotController.getInstance().getFreeSlots(ctx));
                        });
                        path("/reservations", () -> {
                            post("", ctx -> ReservationController.getInstance().addDetail(ctx));
                            get("/{email}", ctx -> ReservationController.getInstance().getUserReservations(ctx));
                        });
                        path("/admin", () -> {
                            path("/prices", () -> {
                                put("/hours", ctx -> AdminController.getInstance().setPrice(ctx, "PARKING"));
                                put("/kw", ctx -> AdminController.getInstance().setPrice(ctx, "CHARGING"));
                            });
                            path("/history", () -> {
                                get("/date", ctx -> AdminController.getInstance().getRequestHistoryByData(ctx));
                                get("/time", ctx -> AdminController.getInstance().getRequestHistoryByTime(ctx));
                                get("", ctx -> AdminController.getInstance().getRequestHistory(ctx));
                                get("/type/{type}", ctx -> AdminController.getInstance().getRequestHistoryByType(ctx));
                                get("/grade/{grade}", ctx -> AdminController.getInstance().getRequestHistoryByGrade(ctx));
                            });
                        });
                        get("/prices", ctx -> PriceController.getInstance().getPrices(ctx));
                        put("/payments/{email}", ctx -> RequestController.getInstance().updatePayment(ctx));
                        path("/requests", () -> {
                            get("/<email>/unpaid", ctx -> RequestController.getInstance().getUnpaidUserRequest(ctx));
                        });
                        get("/queue/size", ctx -> QueueController.getInstance().getQueueSize(ctx));
                        path("/fines", () -> {
                            get("", ctx -> FineController.getInstance().getFines(ctx));
                            get("/{email}", ctx -> FineController.getInstance().getUserFines(ctx));
                            post("", ctx -> FineController.getInstance().addFine(ctx));
                            put("", ctx -> FineController.getInstance().updateFine(ctx));
                        });
                    });
                });
            });
            config.showJavalinBanner = false;
            config.bundledPlugins.enableCors(cors -> {
                cors.addRule(it -> {
                    it.allowHost("http://localhost:3000", "http://localhost:9999", "https://editor.swagger.io");
                });
            });
        };
    }
}
