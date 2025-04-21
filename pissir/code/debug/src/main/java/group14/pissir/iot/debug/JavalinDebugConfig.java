package group14.pissir.iot.debug;

import java.util.function.Consumer;

import group14.pissir.util.schema.newMobile;
import io.javalin.apibuilder.ApiBuilder;
import io.javalin.http.Context;
import io.javalin.http.staticfiles.Location;
import io.javalin.util.JavalinLogger;

public class JavalinDebugConfig {

	public static Consumer<io.javalin.config.JavalinConfig> get() {
		return config -> {
			// config.staticFiles.add("/public"); // when the debug website is completed
			config.staticFiles.add(staticFiles -> { // for faster development html/js-side
				// staticFiles.directory = "./code/debug/src/main/resources/public"; // DEBUG
				staticFiles.directory = "src/main/resources/public";
				staticFiles.location = Location.EXTERNAL;
			});
			config.router.apiBuilder(() -> {
				ApiBuilder.before(ctx -> JavalinLogger.info(ctx.req().getMethod() + " " + ctx.fullUrl()));
				ApiBuilder.get("", ctx -> ctx.redirect("/manager.html"));
				ApiBuilder.post("/setupNewMobile", ctx -> {
					createNewMobile(ctx);
				});
			});
			config.showJavalinBanner = false;
		};
	}

	private static void createNewMobile(Context ctx) {
		newMobile mobile = ctx.bodyAsClass(newMobile.class);

		new MobileJarRunner(mobile.phoneNumber());
	}
}
