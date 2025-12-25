using Autoservice156MAUI.ViewModels.Vehicle;

namespace Autoservice156MAUI.Views.Vehicle
{
    public partial class VehicleListPage : ContentPage
    {
        public VehicleListPage(VehicleListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is VehicleListViewModel viewModel)
            {
                viewModel.OnAppearing();
            }
        }
    }
}