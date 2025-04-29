namespace Backend.Models;

public class Reservation
{
    public string UserEmail { get; set; }
    public int SlotId { get; set; }
    public DateTime DateTimeStart { get; set; }
    public DateTime DateTimeEnd { get; set; }

    // Navigation properties
    public User User { get; set; }
    public Slot Slot { get; set; }
}
