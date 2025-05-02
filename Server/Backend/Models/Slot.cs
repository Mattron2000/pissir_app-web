namespace Backend.Models;

public partial class Slot
{
    public int Id { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual SlotsStatus StatusNavigation { get; set; } = null!;
}
