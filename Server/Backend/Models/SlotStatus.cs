namespace Backend.Models;

public class SlotStatus
{
    public SlotStatusEnum Name { get; set; }
}

public enum SlotStatusEnum
{
    FREE,
    OCCUPIED
}
