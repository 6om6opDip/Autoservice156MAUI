using System.Text.Json.Serialization;

namespace Autoservice156MAUI.Models.DTO;

public class VehicleDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("brand")]
    public string Brand { get; set; } = string.Empty;

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("licensePlate")]
    public string LicensePlate { get; set; } = string.Empty;

    [JsonPropertyName("vin")]
    public string VIN { get; set; } = string.Empty;

    [JsonPropertyName("clientId")]
    public int ClientId { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    public string FullName => $"{Brand} {Model} ({Year})";
}