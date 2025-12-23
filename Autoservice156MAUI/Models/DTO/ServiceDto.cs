using System.Text.Json.Serialization;

namespace Autoservice156MAUI.Models.DTO;

public class ServiceDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("durationMinutes")]
    public int DurationMinutes { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}