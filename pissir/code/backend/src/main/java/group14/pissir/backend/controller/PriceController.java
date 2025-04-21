package group14.pissir.backend.controller;

import java.util.ArrayList;
import java.util.HashMap;

import group14.pissir.backend.service.PriceService;
import group14.pissir.util.schema.PricesData;
import group14.pissir.util.schema.ServicePriceData;
import io.javalin.http.Context;
import io.javalin.openapi.HttpMethod;
import io.javalin.openapi.OpenApi;
import io.javalin.openapi.OpenApiContent;
import io.javalin.openapi.OpenApiResponse;

public class PriceController {

    private static PriceController instance = null;

    public static PriceController getInstance() {
        if (instance == null)
            return new PriceController();
        else
            return instance;
    }

    public PriceController() {
        instance = this;
    }

    // #region
    @OpenApi(
        path = "/prices",
        methods = HttpMethod.GET,
        versions = { "1.0.0" },
        tags = { "Admin" },
        summary = "get prices of parking and charging services",
        description = "Returns the prices of parking and charging services",
        responses = {
            @OpenApiResponse(
                status = "200",
                description = "The prices of parking and charging services",
                content = @OpenApiContent(from = PricesData.class)
            )
        }
    )
    // #endregion
    public void getPrices(Context ctx) {
        ArrayList<ServicePriceData> pricesData = PriceService.getInstance().getPrices();

        HashMap<String, Float> prices = new HashMap<>();
        prices.put("costo_kw", pricesData.get(0).price());
        prices.put("costo_sosta", pricesData.get(1).price());

        ctx.status(200).json(prices);
        // RESPONSE
        //      200
        // {
        //     "costo_kw": 0.0,
        //     "costo_sosta": 0.0
        // }
    }
}
