namespace Backend.Models;

public class SlotStatus
{
    public SlotStatusEnum Name { get; set; }

    // Navigation properties
    public ICollection<Slot> Slots { get; set; }
}

public enum SlotStatusEnum
{
    FREE,
    OCCUPIED
}
