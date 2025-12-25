using System.Collections.ObjectModel;
using Autoservice156MAUI.Models.DTO;

namespace Autoservice156MAUI.Views.Client;

public partial class ClientListPage : ContentPage
{
    public ObservableCollection<ClientDto> Clients { get; } = new();

    public ClientListPage()
    {
        InitializeComponent();
        LoadClients();
    }

    private void LoadClients()
    {
        // Тестовые данные для Windows
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
            // Windows-специфичное действие - можно открыть детали в диалоге
            var answer = await DisplayActionSheet(
                $"Клиент: {client.FullName}",
                "Отмена",
                null,
                "Просмотреть детали",
                "Редактировать",
                "Показать транспорт",
                "Создать заказ");

            if (answer == "Просмотреть детали")
            {
                await DisplayAlert("Детали клиента",
                    $"Имя: {client.FullName}\nEmail: {client.Email}\nТелефон: {client.Phone}\nАдрес: {client.Address}",
                    "OK");
            }

            ClientsCollection.SelectedItem = null;
        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        // Для Windows можно использовать более сложные диалоги
        var name = await DisplayPromptAsync("Новый клиент", "Введите имя клиента:");

        if (!string.IsNullOrWhiteSpace(name))
        {
            var email = await DisplayPromptAsync("Новый клиент", "Введите email:", initialValue: "@example.com");
            var phone = await DisplayPromptAsync("Новый клиент", "Введите телефон:");

            var newClient = new ClientDto
            {
                Id = Clients.Count + 1,
                FirstName = name,
                Email = email ?? "",
                Phone = phone ?? ""
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
}