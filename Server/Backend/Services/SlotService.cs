
using Backend.Models;
using Backend.Repositories;
using Shared.DTOs.Slot;

namespace Backend.Services;

public enum SlotResultEnum
{
    Success,
    Failed
}

public class SlotResponse
{
    public SlotResultEnum Result { get; set; }
    public string? ErrorMessage { get; set; }
    public SlotEntityDTO[]? Slots { get; internal set; }

    public static SlotResponse Success(SlotEntityDTO[] Slots) =>
        new()
        {
            Result = SlotResultEnum.Success,
            Slots = Slots
        };

    public static SlotResponse Failed(SlotResultEnum result = SlotResultEnum.Failed, string? reason = null) =>
        new()
        {
            Result = result,
            ErrorMessage = reason ?? result switch
            {
                SlotResultEnum.Failed => "Failed",
                _ => null
            }
        };
}

public class SlotService(ISlotRepository slotRepository)
{
    private readonly ISlotRepository _slotRepository = slotRepository;

    internal async Task<SlotResponse> GetSlotsAsync()
    {
        Slot[]? Slots = await _slotRepository.GetSlotsAsync();

        if (Slots == null)
            return SlotResponse.Failed();

        return SlotResponse.Success(
            [.. Slots.Select(s => new SlotEntityDTO(s.Id, s.Status))]
        );
    }
}
