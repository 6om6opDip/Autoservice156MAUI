using Microsoft.Maui.Graphics;

namespace Autoservice156MAUI.Models.DTO
{
    public class ServiceDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = "Active"; // "Active" или "Inactive"

        public string StatusText => Status == "Active" ? "Активна" : "Неактивна";
        public Color StatusColor => Status == "Active"
            ? Color.FromArgb("#4CAF50")  // Зеленый
            : Color.FromArgb("#F44336"); // Красный

        public string PriceFormatted => $"{Price}₽";
        public string DurationFormatted => $"{DurationMinutes} мин.";
    }
}