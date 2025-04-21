package group14.pissir.backend;

import group14.pissir.backend.service.FineService;
import group14.pissir.backend.service.ReservationService;
import group14.pissir.util.schema.EmailDatetimesData;
import io.javalin.util.JavalinLogger;

public class ReservationToFine implements Runnable {

    private String email;
    private String datetime_start;
    private String datetime_end;


    public ReservationToFine(EmailDatetimesData reservation) {
        this.email = reservation.email();
        this.datetime_start = reservation.datetime_start();
        this.datetime_end = reservation.datetime_end();
    }

    @Override
    public void run() {
        if (ReservationService.getInstance().deleteReservation(this.email, this.datetime_start.toString())) {
            JavalinLogger.info("Reservation deleted: " + this.email + " " + this.datetime_start.toString());

            if (FineService.getInstance().addFine(this.email, this.datetime_start, this.datetime_end))
                JavalinLogger.info("Fine added: " + this.email + " " + this.datetime_start.toString());
            else
                JavalinLogger.warn("Fine not added: " + this.email + " " + this.datetime_start.toString());
        } else {
            JavalinLogger.info("Reservation not deleted: " + this.email + " " + this.datetime_start.toString());
        }
    }
}
