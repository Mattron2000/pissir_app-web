using Backend.Models;

namespace Backend.Repositories;

public interface ISlotRepository
{
    Task<Slot[]?> GetSlotsAsync();
}
