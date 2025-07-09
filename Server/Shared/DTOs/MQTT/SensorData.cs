using System.Text.Json.Serialization;

namespace Shared.DTOs.MQTT;

public class SensorData
{
    [JsonPropertyName("slot_id")]
    public int SlotId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
