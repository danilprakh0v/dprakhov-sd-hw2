/*
 * Проект: FinanceTracker
 * Файл: MenuItem.cs
 * Расположение: FinanceTracker.ConsoleApp/UI/
 * Назначение: Представляет класс элемента в консольном меню.
 * ==========================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
namespace FinanceTracker.ConsoleApp.UI;

/// <summary>
/// Класс-модель, представляющий один пункт в объекте Menu.
/// Хранит всю необходимую информацию для отображения и выполнения действия.
/// </summary>
public class MenuItem
{
    public string Text { get; }
    public Action? Action { get; }
    public Menu? SubMenu { get; }
    public Func<bool>? IsEnabledDelegate { get; }
    public bool IsSeparator { get; }

    public bool IsEnabled => IsEnabledDelegate?.Invoke() ?? true;

    public MenuItem(string text, Action? action = null, Menu? subMenu = null, Func<bool>? isEnabled = null, bool isSeparator = false)
    {
        Text = text;
        Action = action;
        SubMenu = subMenu;
        IsEnabledDelegate = isEnabled;
        IsSeparator = isSeparator;
    }
}