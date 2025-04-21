package group14.pissir.backend;

import java.time.Duration;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;

import group14.pissir.backend.service.FineService;
import group14.pissir.backend.service.ReservationService;
import group14.pissir.util.schema.EmailDatetimesData;
import io.javalin.util.JavalinLogger;

public class ReservationChecker {

    private static ReservationChecker instance = null;

    private ScheduledExecutorService scheduler = Executors.newScheduledThreadPool(100);

    public static ReservationChecker getInstance() {
		if (instance == null)
			return new ReservationChecker();
		else
			return instance;
	}

    public ReservationChecker() {
        this.schedule();
    }

    private void schedule() {
        ArrayList<EmailDatetimesData> reservations = ReservationService.getInstance().getReservations();
        System.out.println("reservations: " + reservations.size());
		schedule(reservations);
    }

    void schedule(ArrayList<EmailDatetimesData> reservations) {
        if (reservations == null || reservations.size() == 0)
            return;

        for (EmailDatetimesData reservation : reservations)
            this.schedule(reservation);
    }

    void schedule(EmailDatetimesData reservation) {
        String datetime_start = reservation.datetime_start().replace(' ', 'T');
        String email = reservation.email();

        LocalDateTime now = LocalDateTime.now();
        LocalDateTime datetime = LocalDateTime.parse(datetime_start).plusMinutes(10);

        if (now.isAfter(datetime)) {
            if (ReservationService.getInstance().deleteReservation(email, reservation.datetime_start())) {
                JavalinLogger.info("Reservation deleted: " + reservation.toString());
                if (FineService.getInstance().addFine(reservation))
                    JavalinLogger.info("Fine added: " + reservation.toString());
                else
                    JavalinLogger.warn("Fine not added: " + reservation.toString());

            } else
                JavalinLogger.warn("Reservation not deleted: " + reservation.toString());

            return;
        } else {
            Duration duration = Duration.between(now, datetime);
            long minutes = duration.toMinutes();

            ScheduledFuture<?> future = scheduler.schedule(new ReservationToFine(reservation), minutes, TimeUnit.MINUTES);

            long delay = future.getDelay(TimeUnit.MINUTES);
            JavalinLogger.info("Reservation scheduled: " + reservation.toString() + " in " + delay + " minutes");
        }
    }

    public static void main(String[] args) {
        ReservationChecker.getInstance();
    }

    public void schedule(String email, String datetime_start, String datetime_end) {
        this.schedule(new EmailDatetimesData(email, datetime_start, datetime_end));
    }
}
