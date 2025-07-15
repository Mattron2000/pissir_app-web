namespace Shared.DTOs.Reservation;

public record ReservationEntityDTO(
    string Email,
    int SlotId,
    DateTime DatetimeStart,
    DateTime DatetimeEnd
);

public record class ReservationCreateDTO()
{
    public string Email { get; set; } = string.Empty;
    public int SlotId { get; set; } = 0;
    public DateTime DatetimeStart { get; set; }
    public DateTime DatetimeEnd { get; set; }
}
