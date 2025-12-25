using Autoservice156MAUI.Models.DTO;
using System.Text.Json;

namespace Autoservice156MAUI.Views.Service
{
    public partial class ServiceEditPage : ContentPage
    {
        public ServiceEditPage()
        {
            InitializeComponent();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                // Получаем данные
                string name = NameEntry.Text?.Trim();
                string description = DescriptionEntry.Text?.Trim();
                string priceText = PriceEntry.Text?.Trim();
                string durationText = DurationEntry.Text?.Trim();
                string category = CategoryEntry.Text?.Trim();

                // Проверяем обязательные поля
                if (string.IsNullOrEmpty(name))
                {
                    await DisplayAlert("Ошибка", "Введите название услуги", "OK");
                    NameEntry.Focus();
                    return;
                }

                // Парсим цену и длительность
                if (!decimal.TryParse(priceText, out decimal price) || price <= 0)
                {
                    await DisplayAlert("Ошибка", "Введите корректную цену", "OK");
                    PriceEntry.Focus();
                    return;
                }

                if (!int.TryParse(durationText, out int duration) || duration <= 0)
                {
                    await DisplayAlert("Ошибка", "Введите корректную длительность", "OK");
                    DurationEntry.Focus();
                    return;
                }

                // Создаем новую услугу
                var newService = new ServiceDto
                {
                    Id = DateTime.Now.Ticks,
                    Name = name,
                    Description = description ?? "Нет описания",
                    Price = price,
                    DurationMinutes = duration,
                    Category = category ?? "Другое",
                    Status = "Active"
                };

                // Сохраняем локально
                SaveServiceToLocalStorage(newService);

                // Показываем сообщение
                await DisplayAlert("Успех",
                    $"Услуга добавлена!\n{newService.Name}\nЦена: {newService.Price}₽",
                    "OK");

                // Очищаем поля
                NameEntry.Text = "";
                DescriptionEntry.Text = "";
                PriceEntry.Text = "";
                DurationEntry.Text = "";
                CategoryEntry.Text = "";

                // Возвращаемся назад
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось сохранить: {ex.Message}", "OK");
            }
        }

        private void SaveServiceToLocalStorage(ServiceDto newService)
        {
            try
            {
                // Загружаем существующие услуги
                var servicesJson = Preferences.Default.Get("local_services", "[]");
                var services = new List<ServiceDto>();

                if (!string.IsNullOrEmpty(servicesJson) && servicesJson != "[]")
                {
                    services = JsonSerializer.Deserialize<List<ServiceDto>>(servicesJson) ?? new List<ServiceDto>();
                }

                // Добавляем новую услугу
                services.Add(newService);

                // Сохраняем обратно
                var updatedJson = JsonSerializer.Serialize(services);
                Preferences.Default.Set("local_services", updatedJson);

                Console.WriteLine($"✅ Услуга сохранена локально. Всего: {services.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка сохранения: {ex.Message}");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}