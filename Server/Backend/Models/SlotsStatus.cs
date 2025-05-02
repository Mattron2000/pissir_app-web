namespace Backend.Models;

public partial class SlotsStatus
{
    public string Status { get; set; } = null!;

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();
}

public enum SlotsStatusEnum
{
    FREE,
    OCCUPIED
}
