using System.Text.Json.Serialization;

namespace Shared.DTOs.MQTT;

public class MWbotResponse
{
    [JsonPropertyName("slotId")]
    public int SlotId { get; set; }

    [JsonPropertyName("kw")]
    public int Kw { get; set; }
}
