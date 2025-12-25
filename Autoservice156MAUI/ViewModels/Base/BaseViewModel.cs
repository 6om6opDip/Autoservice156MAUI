using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Autoservice156MAUI.ViewModels.Base;

public abstract class BaseViewModel : INotifyPropertyChanged
{
    private bool _isBusy;
    private string _title = string.Empty;
    private string _errorMessage = string.Empty;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsNotBusy => !IsBusy;

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;

        storage = value;
        OnPropertyChanged(propertyName);

        if (propertyName == nameof(IsBusy))
        {
            OnPropertyChanged(nameof(IsNotBusy));
        }

        return true;
    }

    protected virtual async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    protected async Task DisplayAlert(string title, string message, string cancel = "OK")
    {
        await Application.Current.MainPage.DisplayAlert(title, message, cancel);
    }

    protected async Task NavigateToAsync(string route, Dictionary<string, object> parameters = null)
    {
        try
        {
            if (Application.Current?.MainPage != null)
            {
                if (parameters != null)
                {
                    await Shell.Current.GoToAsync(route, parameters);
                }
                else
                {
                    await Shell.Current.GoToAsync(route);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка навигации: {ex.Message}");
        }
    }
}