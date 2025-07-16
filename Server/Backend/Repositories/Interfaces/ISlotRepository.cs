using Backend.Models;

namespace Backend.Repositories.Interfaces;

public interface ISlotRepository
{
    Task<Slot[]> GetSlotsAsync();
    Task<bool> UpdateSlotAsync(Slot slot);
}
