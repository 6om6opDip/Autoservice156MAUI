public class VehicleDto
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string VIN { get; set; } = string.Empty;

    public string FullName => $"{Brand} {Model}";
    public string FullInfo => $"{Brand} {Model} ({LicensePlate})";
}