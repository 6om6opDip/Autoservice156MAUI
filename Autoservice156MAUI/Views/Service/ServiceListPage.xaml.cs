using Autoservice156MAUI.ViewModels.Service;

namespace Autoservice156MAUI.Views.Service
{
    public partial class ServiceListPage : ContentPage
    {
        public ServiceListPage(ServiceListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void OnAddServiceClicked(object sender, EventArgs e)
        {
            try
            {
                var editPage = new ServiceEditPage();
                await Navigation.PushAsync(editPage);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is ServiceListViewModel viewModel) viewModel.OnAppearing();
        }
    }
}