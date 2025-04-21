package group14.pissir.util.schema;

public record NewReservationData(String email, int slot_id, String datetime_start, String datetime_end, Integer percentage, boolean reservation, String phone_number) {}
