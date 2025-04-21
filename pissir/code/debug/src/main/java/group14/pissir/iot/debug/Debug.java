package group14.pissir.iot.debug;

import java.util.Scanner;

import io.javalin.Javalin;

public class Debug {

	public static void main(String[] args) {
		Scanner scanner = new Scanner(System.in);

		Integer port = 8080;

		Javalin javalin = Javalin.create(JavalinDebugConfig.get());

		javalin.start(port);

		System.out.println("Debug server started on port " + port);

		System.out.println("Press [Q] to quit");
		String input;
		do {
			input = scanner.nextLine();

			if (input.equals("Q") || input.equals("q"))
				break;
		} while (true);

		scanner.close();
		javalin.stop();
	}
}
