/*
 * Проект: FinanceTracker
 * Файл: Menu.cs
 * Расположение: FinanceTracker.ConsoleApp/UI/
 * Назначение: Класс для создания и управления интерактивным консольным меню.
 * ==========================================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
namespace FinanceTracker.ConsoleApp.UI;

/// <summary>
/// Инкапсулирует логику для создания, отображения и управления интерактивным консольным меню.
/// </summary>
public class Menu
{
    private readonly List<MenuItem> _items = new();
    private int _selectedIndex;
    private readonly string _title;
    private readonly bool _isMainMenu;

    public Menu(string title, bool isMainMenu = false)
    {
        _title = title;
        _isMainMenu = isMainMenu;
    }

    public void AddItem(string text, Action? action = null, Menu? subMenu = null, Func<bool>? isEnabled = null)
    {
        _items.Add(new MenuItem(text, action, subMenu, isEnabled));
    }

    public void AddSeparator()
    {
        _items.Add(new MenuItem("", isSeparator: true));
    }

    public void Display()
    {
        if (!_items.Any(it => it.IsEnabled && !it.IsSeparator)) return;

        while (!_items[_selectedIndex].IsEnabled || _items[_selectedIndex].IsSeparator)
        {
            _selectedIndex = (_selectedIndex + 1) % _items.Count;
        }

        while (true)
        {
            Console.Clear();
            DrawMenu();
            ConsoleKeyInfo key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow: MoveSelection(-1); break;
                case ConsoleKey.DownArrow: MoveSelection(1); break;
                case ConsoleKey.Enter:
                    var selectedItem = _items[_selectedIndex];
                    if (!selectedItem.IsEnabled) continue;
                    if (selectedItem.Action == GoBackAction) return;

                    Console.Clear();
                    selectedItem.Action?.Invoke();
                    selectedItem.SubMenu?.Display();

                    if (selectedItem.Action != null && selectedItem.SubMenu == null)
                    {
                        Console.Write("\nНажмите любую клавишу для возврата...");
                        Console.ReadKey(true);
                    }
                    break;
                case ConsoleKey.Escape:
                    if (_isMainMenu) ExitProgram();
                    return;
            }
        }
    }

    private void MoveSelection(int direction)
    {
        if (!_items.Any(it => it.IsEnabled && !it.IsSeparator)) return;
        int originalIndex = _selectedIndex;
        do
        {
            _selectedIndex = (_selectedIndex + direction + _items.Count) % _items.Count;
        } while ((!_items[_selectedIndex].IsEnabled || _items[_selectedIndex].IsSeparator) && _selectedIndex != originalIndex);
    }

    public static void GoBackAction() { }

    private void DrawMenu()
    {
        const int width = 60;
        ConsoleUI.DrawBoxedHeader(_title);

        for (int i = 0; i < _items.Count; i++)
        {
            var item = _items[i];
            Console.Write("║");

            if (item.IsSeparator)
            {
                Console.Write("".PadRight(width - 2));
            }
            else
            {
                string text = " " + item.Text;
                if (i == _selectedIndex)
                {
                    Console.Write("    ");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write("o-->");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" ✓ ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(text.PadRight(width - 13));
                }
                else
                {
                    Console.ForegroundColor = item.IsEnabled ? ConsoleColor.White : ConsoleColor.DarkGray;
                    Console.Write("".PadRight(10) + text.PadRight(width - 12));
                }
                Console.ResetColor();
            }
            Console.WriteLine("║");
        }
        Console.WriteLine("╚" + new string('═', width - 2) + "╝");
        Console.WriteLine("\nНаведитесь на соответствующий пункт и нажмите Enter:");
    }

    public static void ExitProgram()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Сеанс работы с «ВШЭ-Банк: Учет финансов» успешно завершён!");
        Console.ResetColor();
        Environment.Exit(0);
    }
}