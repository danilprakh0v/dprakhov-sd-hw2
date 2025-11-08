/*
 * Проект: FinanceTracker
 * Файл: AppHost.cs
 * Расположение: FinanceTracker.ConsoleApp/
 * Назначение: Класс-управитель, контролирующий жизненный цикл консольного приложения.
 * ===================================================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
using FinanceTracker.ConsoleApp.UI;

namespace FinanceTracker.ConsoleApp;

/// <summary>
/// Корневой класс приложения (Composition Root), ответственный за инициализацию
/// основных сервисов и запуск главного цикла приложения.
/// </summary>
public class AppHost
{
    private readonly IConsoleUI _consoleUI;

    public AppHost(IConsoleUI consoleUI)
    {
        _consoleUI = consoleUI;
    }

    public void Run()
    {
        _consoleUI.PrepopulateData();
        _consoleUI.RunMainMenu();
    }
}