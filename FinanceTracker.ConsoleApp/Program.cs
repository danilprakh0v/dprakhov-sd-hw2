/*
 * Проект: FinanceTracker
 * Файл: Program.cs
 * Расположение: FinanceTracker.ConsoleApp/
 * Назначение: Точка входа в приложение.
 * ==============================================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Application.Services;
using FinanceTracker.Core.Interfaces;
using FinanceTracker.Infrastructure.IO;
using FinanceTracker.Infrastructure.Repositories;
using FinanceTracker.ConsoleApp.UI;

namespace FinanceTracker.ConsoleApp;

/// <summary>
/// Главный класс приложения, содержащий точку входа - (Main).
/// Отвечает за конфигурацию и построение контейнера внедрения зависимостей - (DI Container).
/// Также настраивает локаль для отображения денежных сумм в рублях (₽).
/// </summary>
public static class Program
{
    /// <summary>
    /// Точка входа в консольное приложение.
    /// </summary>
    /// <param name="args"> Аргументы командной строки (в данном приложении не используются). </param>
    public static void Main(string[] args)
    {
        // Установка русского параметра для CultureInfo - для форматирования денежных сумм как "1 234,50 ₽"
        var culture = CultureInfo.CreateSpecificCulture("ru-RU");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        // Настройка консоли
        Console.Title = "ВШЭ-Банк : Учет финансов";
        Console.OutputEncoding = Encoding.UTF8;

        // Конфигурация DI-контейнера
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();

        // Запуск приложения
        var app = serviceProvider.GetRequiredService<AppHost>();
        app.Run();
    }

    /// <summary>
    /// Конфигурирует коллекцию служб, регистрируя все необходимые зависимости приложения.
    /// </summary>
    /// <param name="services"> Коллекция служб для конфигурации. </param>
    private static void ConfigureServices(IServiceCollection services)
    {
        // Infrastructure
        services.AddSingleton<IBankAccountRepository, InMemoryBankAccountRepository>();
        services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
        services.AddSingleton<IOperationRepository, InMemoryOperationRepository>();
        services.AddSingleton<IFileImportExportService, FileImportExportService>();

        // Application
        services.AddSingleton<IAccountService, AccountService>();
        services.AddSingleton<ICategoryService, CategoryService>();
        services.AddSingleton<IOperationService, OperationService>();
        services.AddSingleton<IAnalyticsService, AnalyticsService>();

        // UI
        services.AddSingleton<IConsoleUI, ConsoleUI>();
        services.AddSingleton<AppHost>();
    }
}