namespace Backend.Models;

public class Slot
{
    public int Id { get; set; }
    public SlotStatusEnum Status { get; set; }

    // Navigation properties
    public SlotStatus SlotStatus { get; set; }
    public List<Reservation> Reservations { get; set; }
}
