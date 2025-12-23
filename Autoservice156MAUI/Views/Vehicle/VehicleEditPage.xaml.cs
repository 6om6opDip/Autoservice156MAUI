using Autoservice156MAUI.ViewModels.Vehicle;

namespace Autoservice156MAUI.Views.Vehicle;

public partial class VehicleEditPage : ContentPage
{
    public VehicleEditPage()
    {
        InitializeComponent();
    }

    public VehicleEditPage(VehicleEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Сохранение", "Транспорт сохранен", "OK");
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}