package group14.pissir.iot.debug;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.function.Consumer;

public class MobileJarRunner {

    private static class StreamGobbler implements Runnable {
        private InputStream inputStream;
        private Consumer<String> consumer;

        public StreamGobbler(InputStream inputStream, Consumer<String> consumer) {
            this.inputStream = inputStream;
            this.consumer = consumer;
        }

        @Override
        public void run() {
            new BufferedReader(new InputStreamReader(inputStream)).lines()
                    .forEach(consumer);
        }
    }

    public MobileJarRunner(String phoneNumber) {
        // URL mobileJar = ClassLoader.getSystemClassLoader().getResource("mobile.jar");

        ProcessBuilder builder = new ProcessBuilder();
        builder.command("java", "-jar", "mobile.jar", phoneNumber, "true");
        // builder.command("java", "-jar", mobileJar.toURI().getPath(), phoneNumber, "true");
        builder.directory(new File("../mobile/build/libs"));

        Process process;
        try {
            process = builder.start();
        } catch (IOException e) {
            System.err.println(e.getMessage());
            System.exit(1);
            return;
        }

        StreamGobbler streamGobbler = new StreamGobbler(process.getInputStream(), System.out::println);
        new Thread(streamGobbler).start();
    }

}
