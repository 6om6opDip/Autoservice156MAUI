using System.Collections.ObjectModel;
using System.Windows.Input;
using Autoservice156MAUI.Models.DTO;
using Autoservice156MAUI.Services.Interfaces;
using Autoservice156MAUI.ViewModels.Base;
using Autoservice156MAUI.Views.Vehicle;

namespace Autoservice156MAUI.ViewModels.Vehicle;

public class VehicleListViewModel : BaseViewModel
{
    private readonly IVehicleService _vehicleService;
    private VehicleDto _selectedVehicle;
    private string _searchText = string.Empty;

    public ObservableCollection<VehicleDto> Vehicles { get; } = new();
    public ObservableCollection<VehicleDto> FilteredVehicles { get; } = new();

    public VehicleDto SelectedVehicle
    {
        get => _selectedVehicle;
        set
        {
            SetProperty(ref _selectedVehicle, value);
            if (value != null)
            {
                SelectVehicle(value);
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
                FilterVehicles();
            }
        }
    }

    public ICommand LoadVehiclesCommand { get; }
    public ICommand AddVehicleCommand { get; }
    public ICommand RefreshCommand { get; }

    public VehicleListViewModel(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
        Title = "Транспортные средства";

        LoadVehiclesCommand = new Command(async () => await LoadVehiclesAsync());
        AddVehicleCommand = new Command(async () => await AddVehicleAsync());
        RefreshCommand = new Command(async () => await RefreshAsync());
    }

    public async Task LoadVehiclesAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var vehicles = await _vehicleService.GetAllVehiclesAsync();

            Vehicles.Clear();
            foreach (var vehicle in vehicles)
            {
                Vehicles.Add(vehicle);
            }

            FilterVehicles();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка загрузки транспорта: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void FilterVehicles()
    {
        FilteredVehicles.Clear();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            foreach (var vehicle in Vehicles)
            {
                FilteredVehicles.Add(vehicle);
            }
        }
        else
        {
            var searchLower = SearchText.ToLower();
            foreach (var vehicle in Vehicles.Where(v =>
                v.FullName.ToLower().Contains(searchLower) ||
                v.LicensePlate.ToLower().Contains(searchLower) ||
                v.Brand.ToLower().Contains(searchLower) ||
                v.Model.ToLower().Contains(searchLower)))
            {
                FilteredVehicles.Add(vehicle);
            }
        }
    }

    private async void SelectVehicle(VehicleDto vehicle)
    {
        var parameters = new Dictionary<string, object>
        {
            { "VehicleId", vehicle.Id }
        };
        await NavigateToAsync(nameof(VehicleDetailsPage), parameters);
        SelectedVehicle = null;
    }

    private async Task AddVehicleAsync()
    {
        await NavigateToAsync(nameof(VehicleEditPage));
    }

    private async Task RefreshAsync()
    {
        await LoadVehiclesAsync();
    }

    public void OnAppearing()
    {
        if (!Vehicles.Any())
        {
            LoadVehiclesCommand.Execute(null);
        }
    }
}