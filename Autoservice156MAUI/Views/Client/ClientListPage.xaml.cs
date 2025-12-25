using Autoservice156MAUI.Models.DTO;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Autoservice156MAUI.Views.Client;

public partial class ClientListPage : ContentPage
{
    public ObservableCollection<ClientDto> Clients { get; } = new();
    private Dictionary<int, List<string>> _clientVehicles = new();

    public ClientListPage()
    {
        InitializeComponent();
        InitializeClientVehicles();
        LoadClients();
    }

    private void InitializeClientVehicles()
    {
        _clientVehicles = new Dictionary<int, List<string>>
        {
            { 1, new List<string> { "Toyota Camry (А123ВС77)", "Honda Civic (Е555КХ77)" } },
            { 2, new List<string> { "BMW X5 (В456ОР77)", "Mercedes C-Class (Н888ММ77)" } },
            { 3, new List<string> { "Lada Vesta (С789МН77)" } },
            { 4, new List<string> { "Volkswagen Golf (Р111АА77)", "Audi A4 (У222УУ77)", "Skoda Octavia (Х333ХХ77)" } },
            { 5, new List<string> { "Kia Rio (Т444ТТ77)" } }
        };
    }

    private void LoadClients()
    {
        Clients.Clear();

        var testClients = new List<ClientDto>
        {
            new() { Id = 1, FirstName = "Иван", LastName = "Петров", Email = "ivan@autoservice.ru", Phone = "+7 (495) 123-45-67", Address = "Москва, ул. Ленина, 10" },
            new() { Id = 2, FirstName = "Мария", LastName = "Сидорова", Email = "maria@autoservice.ru", Phone = "+7 (495) 234-56-78", Address = "Москва, ул. Мира, 25" },
            new() { Id = 3, FirstName = "Алексей", LastName = "Кузнецов", Email = "alex@autoservice.ru", Phone = "+7 (495) 345-67-89", Address = "Москва, пр. Победы, 15" },
            new() { Id = 4, FirstName = "ООО 'АвтоСтар'", LastName = "", Email = "info@autostar.ru", Phone = "+7 (495) 456-78-90", Address = "Москва, ул. Промышленная, 5" },
            new() { Id = 5, FirstName = "ИП", LastName = "Смирнов Д.А.", Email = "smirnov@service.ru", Phone = "+7 (495) 567-89-01", Address = "Москва, ул. Центральная, 33" },
        };

        foreach (var client in testClients)
        {
            Clients.Add(client);
        }

        ClientsCollection.ItemsSource = Clients;
    }

    private async void OnClientSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ClientDto client)
        {
            var answer = await DisplayActionSheet(
                $"Клиент: {client.FullName}",
                "Отмена",
                "Удалить",
                "Просмотреть детали",
                "Редактировать",
                "Показать транспорт",
                "Добавить транспорт",

                "Создать заказ");

            if (answer == "Удалить")
            {
                await DeleteClient(client);
            }
            else if (answer == "Просмотреть детали")
            {
                await DisplayAlert("Детали клиента",
                    $"Имя: {client.FullName}\nEmail: {client.Email}\nТелефон: {client.Phone}\nАдрес: {client.Address}",
                    "OK");
            }
            else if (answer == "Редактировать")
            {
                await EditClient(client);
            }
            else if (answer == "Показать транспорт")
            {
                await ShowClientVehicles(client);
            }
            else if (answer == "Создать заказ")
            {
                await CreateOrder(client);
            }

            ClientsCollection.SelectedItem = null;
        }
    }

    private async Task DeleteClient(ClientDto client)
    {
        bool confirm = await DisplayAlert("Удаление", $"Удалить клиента {client.FullName}?", "Да", "Нет");

        if (confirm)
        {
            Clients.Remove(client);
            _clientVehicles.Remove(client.Id);
            await DisplayAlert("Успех", "Клиент удалён", "OK");
        }
    }

    private async Task ShowClientVehicles(ClientDto client)
    {
        if (_clientVehicles.TryGetValue(client.Id, out var vehicles))
        {
            var vehiclesText = string.Join("\n", vehicles.Select(v => $"• {v}"));
            await DisplayAlert($"Транспорт клиента {client.FullName}",
                $"Всего транспорта: {vehicles.Count}\n\n{vehiclesText}", "OK");
        }
        else
        {
            await DisplayAlert($"Транспорт клиента {client.FullName}",
                "У клиента нет зарегистрированного транспорта", "OK");
        }
    }

    private async Task EditClient(ClientDto client)
    {
        var newName = await DisplayPromptAsync("Редактирование", "Введите новое имя:", initialValue: client.FullName);

        if (!string.IsNullOrWhiteSpace(newName))
        {
            var newEmail = await DisplayPromptAsync("Редактирование", "Введите новый email:", initialValue: client.Email);
            var newPhone = await DisplayPromptAsync("Редактирование", "Введите новый телефон:", initialValue: client.Phone);

            client.FirstName = newName;
            client.Email = newEmail ?? client.Email;
            client.Phone = newPhone ?? client.Phone;

            ClientsCollection.ItemsSource = null;
            ClientsCollection.ItemsSource = Clients;

            await DisplayAlert("Успех", "Клиент обновлён", "OK");
        }
    }

    private async Task CreateOrder(ClientDto client)
    {
        var serviceType = await DisplayActionSheet(
            "Тип услуги",
            "Отмена",
            null,
            "Замена масла - 2500₽",
            "Диагностика - 1500₽",
            "Шиномонтаж - 3000₽",
            "Ремонт - 5000₽");

        if (serviceType != null && serviceType != "Отмена")
        {
            var confirm = await DisplayAlert("Подтверждение",
                $"Создать заказ для {client.FullName}?\nУслуга: {serviceType}",
                "Да", "Нет");

            if (confirm)
            {
                await DisplayAlert("Успех", $"Заказ создан для {client.FullName}", "OK");
            }
        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync("Новый клиент", "Введите имя клиента:");

        if (!string.IsNullOrWhiteSpace(name))
        {
            var email = await DisplayPromptAsync("Новый клиент", "Введите email:", initialValue: "@example.com");
            var phone = await DisplayPromptAsync("Новый клиент", "Введите телефон:");
            var address = await DisplayPromptAsync("Новый клиент", "Введите адрес:");

            var newClient = new ClientDto
            {
                Id = Clients.Count + 1,
                FirstName = name,
                Email = email ?? "",
                Phone = phone ?? "",
                Address = address ?? ""
            };

            Clients.Add(newClient);
            await DisplayAlert("Успех", "Клиент добавлен", "OK");
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.ToLower() ?? "";

        if (string.IsNullOrWhiteSpace(searchText))
        {
            ClientsCollection.ItemsSource = Clients;
        }
        else
        {
            var filtered = Clients.Where(c =>
                c.FullName.ToLower().Contains(searchText) ||
                c.Email.ToLower().Contains(searchText) ||
                (c.Phone?.Contains(searchText) ?? false)
            ).ToList();

            ClientsCollection.ItemsSource = filtered;
        }
    }

    private async Task AddVehicleToClient(ClientDto client)
    {
        var brand = await DisplayPromptAsync("Добавить транспорт", "Марка автомобиля:");
        if (string.IsNullOrWhiteSpace(brand)) return;

        var model = await DisplayPromptAsync("Добавить транспорт", "Модель автомобиля:");
        if (string.IsNullOrWhiteSpace(model)) return;

        var licensePlate = await DisplayPromptAsync("Добавить транспорт", "Госномер:");

        // Добавляем в словарь
        if (!_clientVehicles.ContainsKey(client.Id))
        {
            _clientVehicles[client.Id] = new List<string>();
        }

        var vehicleInfo = $"{brand} {model} ({licensePlate})";
        _clientVehicles[client.Id].Add(vehicleInfo);

        // Также можно добавить в общую базу транспорта
        AddToGlobalVehicles(client.Id, brand, model, licensePlate);

        await DisplayAlert("Успех", $"Транспорт добавлен клиенту {client.FullName}", "OK");
    }

    private void AddToGlobalVehicles(long clientId, string brand, string model, string licensePlate)
    {
        var vehiclesJson = Preferences.Default.Get("global_vehicles", "[]");
        var vehicles = new List<VehicleDto>();

        if (!string.IsNullOrEmpty(vehiclesJson) && vehiclesJson != "[]")
        {
            vehicles = JsonSerializer.Deserialize<List<VehicleDto>>(vehiclesJson) ?? new List<VehicleDto>();
        }

        vehicles.Add(new VehicleDto
        {
            Id = DateTime.Now.Ticks,
            ClientId = clientId, // Теперь long
            Brand = brand,
            Model = model,
            LicensePlate = licensePlate ?? "Не указан",
            Year = DateTime.Now.Year
        });

        var updatedJson = JsonSerializer.Serialize(vehicles);
        Preferences.Default.Set("global_vehicles", updatedJson);
    }

}