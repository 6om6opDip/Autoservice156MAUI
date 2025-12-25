using Autoservice156MAUI.Models.DTO;
using System.Text.Json;

namespace Autoservice156MAUI.Views.Vehicle
{
	public partial class VehicleEditPage : ContentPage
	{
		private bool _hasUnsavedChanges = false;

		public VehicleEditPage()
		{
			InitializeComponent();

			// Устанавливаем фокус на первое поле при загрузке
			BrandEntry.Focus();

			// Отслеживаем изменения
			BrandEntry.TextChanged += OnEntryTextChanged;
			ModelEntry.TextChanged += OnEntryTextChanged;
			YearEntry.TextChanged += OnEntryTextChanged;
			LicensePlateEntry.TextChanged += OnEntryTextChanged;
			VINEntry.TextChanged += OnEntryTextChanged;
		}

		private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
		{
			_hasUnsavedChanges = true;
		}

		// Кнопка "←" в шапке
		private async void OnBackButtonClicked(object sender, EventArgs e)
		{
			Console.WriteLine("← Нажата кнопка НАЗАД в шапке");
			await CheckAndClose();
		}

		// Кнопка "Отмена"
		private async void OnCancelClicked(object sender, EventArgs e)
		{
			Console.WriteLine("🚫 Нажата кнопка Отмена");
			await CheckAndClose();
		}

		private async Task CheckAndClose()
		{
			// Проверяем есть ли несохраненные изменения
			if (_hasUnsavedChanges)
			{
				bool answer = await DisplayAlert(
					"Подтверждение",
					"Есть несохраненные изменения. Выйти без сохранения?",
					"Да", "Нет");

				if (!answer) return;
			}

			// Закрываем через Shell (так как открывали через Shell)
			await Shell.Current.GoToAsync("..");
		}

		private bool ValidateForm()
		{
			bool isValid = true;
			string errorMessage = "";

			if (string.IsNullOrWhiteSpace(BrandEntry.Text))
			{
				errorMessage += "• Введите марку транспорта\n";
				isValid = false;
			}

			if (string.IsNullOrWhiteSpace(ModelEntry.Text))
			{
				errorMessage += "• Введите модель транспорта\n";
				isValid = false;
			}

			if (!string.IsNullOrWhiteSpace(YearEntry.Text))
			{
				if (!int.TryParse(YearEntry.Text, out int year) ||
					year < 1900 ||
					year > DateTime.Now.Year + 1)
				{
					errorMessage += "• Введите корректный год (1900-текущий)\n";
					isValid = false;
				}
			}

			if (!string.IsNullOrWhiteSpace(VINEntry.Text) &&
				VINEntry.Text.Trim().Length != 17)
			{
				errorMessage += "• VIN должен содержать 17 символов\n";
				isValid = false;
			}

			if (!isValid)
			{
				DisplayAlert("Ошибка заполнения", errorMessage.Trim(), "OK");
			}

			return isValid;
		}

		private async void OnSaveClicked(object sender, EventArgs e)
		{
			try
			{
				if (!ValidateForm())
				{
					return;
				}

				// Получаем данные
				string brand = BrandEntry.Text?.Trim();
				string model = ModelEntry.Text?.Trim();
				string yearText = YearEntry.Text?.Trim();
				string licensePlate = LicensePlateEntry.Text?.Trim()?.ToUpper();
				string vin = VINEntry.Text?.Trim()?.ToUpper();

				// Парсим год
				int year = DateTime.Now.Year;
				if (!string.IsNullOrEmpty(yearText) && int.TryParse(yearText, out int parsedYear))
				{
					year = parsedYear;
				}

				// Создаем транспорт
				var newVehicle = new VehicleDto
				{
					Id = GenerateId(),
					Brand = brand,
					Model = model,
					Year = year,
					LicensePlate = string.IsNullOrEmpty(licensePlate) ? "Не указан" : licensePlate,
					VIN = string.IsNullOrEmpty(vin) ? "Не указан" : vin
				};

				// Сохраняем локально
				SaveVehicleToLocalStorage(newVehicle);

				// Показываем сообщение
				await DisplayAlert("Успех",
					$"Транспорт сохранен!\n" +
					$"{newVehicle.Brand} {newVehicle.Model}\n" +
					$"Гос. номер: {newVehicle.LicensePlate}",
					"OK");

				// Сбрасываем флаг
				_hasUnsavedChanges = false;

				// Закрываем страницу через Shell
				await Shell.Current.GoToAsync("..");

			}
			catch (Exception ex)
			{
				await DisplayAlert("Ошибка", $"Не удалось сохранить: {ex.Message}", "OK");
			}
		}

		private int GenerateId()
		{
			return Math.Abs(Guid.NewGuid().GetHashCode());
		}

		private void SaveVehicleToLocalStorage(VehicleDto newVehicle)
		{
			try
			{
				var vehiclesJson = Preferences.Default.Get("local_vehicles", "[]");
				var vehicles = new List<VehicleDto>();

				if (!string.IsNullOrEmpty(vehiclesJson) && vehiclesJson != "[]")
				{
					try
					{
						vehicles = JsonSerializer.Deserialize<List<VehicleDto>>(vehiclesJson)
							?? new List<VehicleDto>();
					}
					catch (JsonException)
					{
						vehicles = new List<VehicleDto>();
					}
				}

				// Проверяем на дубликаты
				bool alreadyExists = vehicles.Any(v =>
					v.Brand == newVehicle.Brand &&
					v.Model == newVehicle.Model &&
					v.LicensePlate == newVehicle.LicensePlate);

				if (alreadyExists)
				{
					throw new Exception("Транспорт с такими данными уже существует");
				}

				// Добавляем и сохраняем
				vehicles.Add(newVehicle);
				var updatedJson = JsonSerializer.Serialize(vehicles);
				Preferences.Default.Set("local_vehicles", updatedJson);

				Console.WriteLine($"✅ Транспорт сохранен: {newVehicle.Brand} {newVehicle.Model}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Ошибка сохранения: {ex.Message}");
				throw;
			}
		}

		// Обработка аппаратной кнопки "Назад"
		protected override bool OnBackButtonPressed()
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				await CheckAndClose();
			});

			return true;
		}

		// Очистка
		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			BrandEntry.TextChanged -= OnEntryTextChanged;
			ModelEntry.TextChanged -= OnEntryTextChanged;
			YearEntry.TextChanged -= OnEntryTextChanged;
			LicensePlateEntry.TextChanged -= OnEntryTextChanged;
			VINEntry.TextChanged -= OnEntryTextChanged;
		}
	}
}