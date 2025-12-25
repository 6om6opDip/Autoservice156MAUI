using Autoservice156MAUI.Views.Vehicle;
using Autoservice156MAUI.Views.Service;
// Добавьте другие using по мере необходимости

namespace Autoservice156MAUI
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();

			// Регистрируем маршруты для навигации
			RegisterRoutes();
		}

		private void RegisterRoutes()
		{
			try
			{
				Console.WriteLine("🚗 Регистрируем маршруты...");

				// Регистрируем все страницы, которые открываются через навигацию
				Routing.RegisterRoute("VehicleEditPage", typeof(VehicleEditPage));
				Console.WriteLine("✅ VehicleEditPage зарегистрирован");
				Routing.RegisterRoute("VehicleDetailsPage", typeof(VehicleDetailsPage));
				Routing.RegisterRoute("ServiceEditPage", typeof(ServiceEditPage));

			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Ошибка регистрации маршрутов: {ex.Message}");
			}
		}
	}
}