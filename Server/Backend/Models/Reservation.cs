namespace Backend.Models;

public partial class Reservation
{
    public string Email { get; set; } = null!;

    public DateTime DatetimeStart { get; set; }

    public DateTime DatetimeEnd { get; set; }

    public int SlotId { get; set; }

    public virtual User EmailNavigation { get; set; } = null!;

    public virtual Slot Slot { get; set; } = null!;
}
