using System.Text.Json.Serialization;

namespace Shared.DTOs.MQTT;

public class MWbotRequest
{
    [JsonPropertyName("slot_id")]
    public int SlotId { get; set; }

    [JsonPropertyName("percentage")]
    public int Percentage { get; set; }

    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; } = string.Empty;
}
