namespace Autoservice156MAUI.Models.Enums;

public enum UserRole
{
    User = 0,
    Employee = 1,
    Admin = 2,
    Manager = 3
}

public static class UserRoleExtensions
{
    public static string GetDisplayName(this UserRole role)
    {
        return role switch
        {
            UserRole.User => "Пользователь",
            UserRole.Employee => "Сотрудник",
            UserRole.Admin => "Администратор",
            UserRole.Manager => "Менеджер",
            _ => "Неизвестно"
        };
    }
}