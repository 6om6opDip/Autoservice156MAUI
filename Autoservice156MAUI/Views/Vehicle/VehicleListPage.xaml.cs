using Autoservice156MAUI.Models.DTO;
using Autoservice156MAUI.ViewModels.Vehicle;

namespace Autoservice156MAUI.Views.Vehicle;

public partial class VehicleListPage : ContentPage
{
    private readonly VehicleListViewModel _viewModel;
    private List<VehicleDto> _vehicles = new();

    public VehicleListPage(VehicleListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        LoadTestData();
    }

    private void LoadTestData()
    {
        _vehicles = new List<VehicleDto>
        {
            new() { Id = 1, Brand = "Toyota", Model = "Camry", Year = 2020, LicensePlate = "А123ВС77", VIN = "JTDBU4EE7D9012345" },
            new() { Id = 2, Brand = "BMW", Model = "X5", Year = 2021, LicensePlate = "В456ОР77", VIN = "WBADT43411G123456" },
            new() { Id = 3, Brand = "Lada", Model = "Vesta", Year = 2022, LicensePlate = "С789МН77", VIN = "XTA210530G1234567" },
            new() { Id = 4, Brand = "Kia", Model = "Rio", Year = 2019, LicensePlate = "Е012КХ77", VIN = "Z94CB41BAHR123456" },
            new() { Id = 5, Brand = "Hyundai", Model = "Creta", Year = 2023, LicensePlate = "М345ТУ77", VIN = "Z8TNC21S0C0123456" },
        };

        VehiclesCollection.ItemsSource = _vehicles;
    }

    private async void OnVehicleSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is VehicleDto vehicle)
        {
            await DisplayAlert("Транспорт",
                $"{vehicle.FullName}\nГосномер: {vehicle.LicensePlate}\nVIN: {vehicle.VIN}",
                "OK");

            VehiclesCollection.SelectedItem = null;
        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Добавление транспорта",
            "Функция будет доступна после интеграции с API", "OK");
    }
}