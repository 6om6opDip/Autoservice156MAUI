using Autoservice156MAUI.Models.DTO;
using System.Text.Json;

namespace Autoservice156MAUI.Views.Vehicle
{
    public partial class VehicleEditPage : ContentPage
    {
        public VehicleEditPage()
        {
            InitializeComponent();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                // 1. Получаем данные
                string brand = BrandEntry.Text?.Trim();
                string model = ModelEntry.Text?.Trim();
                string yearText = YearEntry.Text?.Trim();
                string licensePlate = LicensePlateEntry.Text?.Trim();
                string vin = VINEntry.Text?.Trim();

                // 2. Проверяем обязательные поля
                if (string.IsNullOrEmpty(brand))
                {
                    await DisplayAlert("Ошибка", "Введите марку транспорта", "OK");
                    BrandEntry.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(model))
                {
                    await DisplayAlert("Ошибка", "Введите модель транспорта", "OK");
                    ModelEntry.Focus();
                    return;
                }

                // 3. Парсим год
                int year = 2024;
                if (!string.IsNullOrEmpty(yearText) && int.TryParse(yearText, out int parsedYear))
                {
                    year = parsedYear;
                }

                // 4. Создаем новый транспорт
                var newVehicle = new VehicleDto
                {
                    Id = (int)DateTime.Now.Ticks, // Временный ID
                    Brand = brand,
                    Model = model,
                    Year = year,
                    LicensePlate = string.IsNullOrEmpty(licensePlate) ? "Не указан" : licensePlate,
                    VIN = string.IsNullOrEmpty(vin) ? "Не указан" : vin
                };

                // 5. Сохраняем локально
                SaveVehicleToLocalStorage(newVehicle);

                // 6. Показываем сообщение
                await DisplayAlert("Успех",
                    $"Транспорт сохранен!\n" +
                    $"{newVehicle.Brand} {newVehicle.Model}\n" +
                    $"Гос. номер: {newVehicle.LicensePlate}",
                    "OK");

                // 7. Очищаем поля
                BrandEntry.Text = "";
                ModelEntry.Text = "";
                YearEntry.Text = "";
                LicensePlateEntry.Text = "";
                VINEntry.Text = "";

                // 8. Возвращаемся назад
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось сохранить: {ex.Message}", "OK");
            }
        }

        private void SaveVehicleToLocalStorage(VehicleDto newVehicle)
        {
            try
            {
                Console.WriteLine("💾 Начинаем сохранение транспорта...");

                // 1. Загружаем существующие транспорты
                var vehiclesJson = Preferences.Default.Get("local_vehicles", "[]");
                Console.WriteLine($"📁 JSON из хранилища: {vehiclesJson}");

                var vehicles = new List<VehicleDto>();

                // 2. Парсим JSON (если он не пустой)
                if (!string.IsNullOrEmpty(vehiclesJson) && vehiclesJson != "[]")
                {
                    try
                    {
                        vehicles = JsonSerializer.Deserialize<List<VehicleDto>>(vehiclesJson) ?? new List<VehicleDto>();
                        Console.WriteLine($"📊 Загружено {vehicles.Count} существующих транспортов");
                    }
                    catch (JsonException jsonEx)
                    {
                        Console.WriteLine($"⚠️ Ошибка парсинга JSON: {jsonEx.Message}");
                        Console.WriteLine($"⚠️ Создаем новый список");
                        vehicles = new List<VehicleDto>();
                    }
                }
                else
                {
                    Console.WriteLine("📁 Хранилище пустое, создаем новый список");
                }

                // 3. Проверяем, нет ли такого же транспорта
                bool alreadyExists = vehicles.Any(v =>
                    v.Brand == newVehicle.Brand &&
                    v.Model == newVehicle.Model &&
                    v.LicensePlate == newVehicle.LicensePlate);

                if (alreadyExists)
                {
                    Console.WriteLine("⚠️ Такой транспорт уже существует");
                    return;
                }

                // 4. Добавляем новый транспорт
                vehicles.Add(newVehicle);
                Console.WriteLine($"➕ Добавлен новый транспорт: {newVehicle.Brand} {newVehicle.Model}");
                Console.WriteLine($"📊 Теперь всего: {vehicles.Count} транспортов");

                // 5. Сохраняем обратно
                var updatedJson = JsonSerializer.Serialize(vehicles);
                Console.WriteLine($"💾 Сохраняем JSON: {updatedJson}");

                Preferences.Default.Set("local_vehicles", updatedJson);

                // 6. Дополнительная проверка
                var savedJson = Preferences.Default.Get("local_vehicles", "[]");
                var savedCount = JsonSerializer.Deserialize<List<VehicleDto>>(savedJson)?.Count ?? 0;

                Console.WriteLine($"✅ Проверка сохранения: {savedCount} транспортов в хранилище");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Критическая ошибка сохранения: {ex.Message}");
                Console.WriteLine($"📌 StackTrace: {ex.StackTrace}");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            // Просто закрываем страницу
            await Navigation.PopAsync();
        }
    }
}