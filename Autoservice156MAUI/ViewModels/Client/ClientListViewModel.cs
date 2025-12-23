using System.Collections.ObjectModel;
using System.Windows.Input;
using Autoservice156MAUI.Models.DTO;
using Autoservice156MAUI.Services.Interfaces;
using Autoservice156MAUI.ViewModels.Base;
using Autoservice156MAUI.Views.Client;

namespace Autoservice156MAUI.ViewModels.Client;

public class ClientListViewModel : BaseViewModel
{
    private readonly IClientService _clientService;
    private ClientDto _selectedClient;
    private string _searchText = string.Empty;

    public ObservableCollection<ClientDto> Clients { get; } = new();
    public ObservableCollection<ClientDto> FilteredClients { get; } = new();

    public ClientDto SelectedClient
    {
        get => _selectedClient;
        set
        {
            SetProperty(ref _selectedClient, value);
            if (value != null)
            {
                SelectClient(value);
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                FilterClients();
            }
        }
    }

    public ICommand LoadClientsCommand { get; }
    public ICommand AddClientCommand { get; }
    public ICommand RefreshCommand { get; }

    public ClientListViewModel(IClientService clientService)
    {
        _clientService = clientService;
        Title = "Клиенты";

        LoadClientsCommand = new Command(async () => await LoadClientsAsync());
        AddClientCommand = new Command(async () => await AddClientAsync());
        RefreshCommand = new Command(async () => await RefreshAsync());
    }

    public async Task LoadClientsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var clients = await _clientService.GetAllClientsAsync();

            Clients.Clear();
            foreach (var client in clients)
            {
                Clients.Add(client);
            }

            FilterClients();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка загрузки клиентов: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void FilterClients()
    {
        FilteredClients.Clear();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            foreach (var client in Clients)
            {
                FilteredClients.Add(client);
            }
        }
        else
        {
            var searchLower = SearchText.ToLower();
            foreach (var client in Clients.Where(c =>
                c.FullName.ToLower().Contains(searchLower) ||
                c.Email.ToLower().Contains(searchLower) ||
                c.Phone.Contains(_searchText)))
            {
                FilteredClients.Add(client);
            }
        }
    }

    private async void SelectClient(ClientDto client)
    {
        var parameters = new Dictionary<string, object>
        {
            { "ClientId", client.Id }
        };
        await NavigateToAsync(nameof(ClientDetailsPage), parameters);
        SelectedClient = null; // Сброс выбора
    }

    private async Task AddClientAsync()
    {
        await NavigateToAsync(nameof(ClientEditPage));
    }

    private async Task RefreshAsync()
    {
        await LoadClientsAsync();
    }

    public void OnAppearing()
    {
        // Загружаем при появлении страницы
        if (!Clients.Any())
        {
            LoadClientsCommand.Execute(null);
        }
    }
}