
using Backend.Models;
using Backend.Repositories.Interfaces;
using Shared.DTOs.Slot;

namespace Backend.Services;

public enum SlotResultEnum
{
    Success,
    Failed,
    NotFound
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
                SlotResultEnum.NotFound => "Slot not found",
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

    internal async Task<SlotResponse> UpdateSlotAsync(int slotId, string? status = null)
    {
        Slot[]? Slots = await _slotRepository.GetSlotsAsync();

        if (Slots == null)
            return SlotResponse.Failed();

        Slot? slot = Slots.FirstOrDefault(s => s.Id == slotId);

        if (slot == null)
            return SlotResponse.Failed(SlotResultEnum.NotFound);

        if (status != null)
            slot.Status = status;
        else
            if (slot.Status == SlotsStatusEnum.FREE.ToString())
                slot.Status = SlotsStatusEnum.OCCUPIED.ToString();
            else
                slot.Status = SlotsStatusEnum.FREE.ToString();

        if (await _slotRepository.UpdateSlotAsync(slot))
            return SlotResponse.Success(
                [new SlotEntityDTO(slot.Id, slot.Status)]
            );

        return SlotResponse.Failed();
    }
}
