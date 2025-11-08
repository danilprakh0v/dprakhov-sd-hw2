/*
 * Проект: FinanceTracker
 * Файл: ConsoleUI.cs
 * Расположение: FinanceTracker.ConsoleApp/UI/
 * Назначение: Консольный интерфейс с измерением времени сценариев, гибким вводом JSON-путей и расширенной статистикой.
 * ====================================================================================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */

using System.Text;
using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Decorators;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Core.Entities;
using FinanceTracker.Infrastructure.IO;

namespace FinanceTracker.ConsoleApp.UI;

/// <summary>
/// Реализует пользовательский интерфейс для консольного приложения.
/// Все ключевые пользовательские сценарии инкапсулированы в ICommand и обёрнуты в TimedCommandDecorator
/// для измерения времени выполнения (требование ДЗ №2, п.4a).
/// Поддерживает гибкий ввод пути для импорта/экспорта JSON, цветовое оформление, иерархическое меню
/// и расширенную статистику по операциям.
/// </summary>
public class ConsoleUI : IConsoleUI
{
    private readonly IAccountService _accountService;
    private readonly ICategoryService _categoryService;
    private readonly IOperationService _operationService;
    private readonly IAnalyticsService _analyticsService;
    private readonly IFileImportExportService _fileService;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ConsoleUI"/> с внедрёнными зависимостями.
    /// </summary>
    public ConsoleUI(
        IAccountService accountService,
        ICategoryService categoryService,
        IOperationService operationService,
        IAnalyticsService analyticsService,
        IFileImportExportService fileService)
    {
        _accountService = accountService;
        _categoryService = categoryService;
        _operationService = operationService;
        _analyticsService = analyticsService;
        _fileService = fileService;
    }

    /// <summary>
    /// Запускает главное меню приложения с иерархической навигацией.
    /// Все сценарии, требующие измерения времени, обёрнуты в TimedCommandDecorator.
    /// </summary>
    public void RunMainMenu()
    {

        // Подменю: Создание счёта
        var createAccountMenu = new Menu("Создание счёта");
        createAccountMenu.AddItem("   → Основной счёт", () => ExecuteTimedCommand(new CreateAccountCommand(_accountService, "Основной счёт")));
        createAccountMenu.AddItem("   → Наличные", () => ExecuteTimedCommand(new CreateAccountCommand(_accountService, "Наличные")));
        createAccountMenu.AddItem("   → Сберегательный", () => ExecuteTimedCommand(new CreateAccountCommand(_accountService, "Сберегательный")));
        createAccountMenu.AddItem("   → Другой", () => ExecuteTimedCommand(new CreateAccountCommand(_accountService, ReadLine("Название счёта"))));
        createAccountMenu.AddSeparator();
        createAccountMenu.AddItem("   → Назад", Menu.GoBackAction);

        // Подменю: Категории доходов
        var incomeCategoryMenu = new Menu("Категории доходов");
        incomeCategoryMenu.AddItem("   → Зарплата", () => ExecuteTimedCommand(new CreateCategoryCommand(_categoryService, CategoryType.Income, "Зарплата")));
        incomeCategoryMenu.AddItem("   → Кэшбэк", () => ExecuteTimedCommand(new CreateCategoryCommand(_categoryService, CategoryType.Income, "Кэшбэк")));
        incomeCategoryMenu.AddItem("   → Подарок", () => ExecuteTimedCommand(new CreateCategoryCommand(_categoryService, CategoryType.Income, "Подарок")));
        incomeCategoryMenu.AddItem("   → Другое", () => ExecuteTimedCommand(new CreateCategoryCommand(_categoryService, CategoryType.Income, ReadLine("Название"))));
        incomeCategoryMenu.AddSeparator();
        incomeCategoryMenu.AddItem("   → Назад", Menu.GoBackAction);

        // Подменю: Категории расходов
        var expenseCategoryMenu = new Menu("Категории расходов");
        expenseCategoryMenu.AddItem("   → Кафе", () => ExecuteTimedCommand(new CreateCategoryCommand(_categoryService, CategoryType.Expense, "Кафе")));
        expenseCategoryMenu.AddItem("   → Здоровье", () => ExecuteTimedCommand(new CreateCategoryCommand(_categoryService, CategoryType.Expense, "Здоровье")));
        expenseCategoryMenu.AddItem("   → Транспорт", () => ExecuteTimedCommand(new CreateCategoryCommand(_categoryService, CategoryType.Expense, "Транспорт")));
        expenseCategoryMenu.AddItem("   → Продукты", () => ExecuteTimedCommand(new CreateCategoryCommand(_categoryService, CategoryType.Expense, "Продукты")));
        expenseCategoryMenu.AddItem("   → Другое", () => ExecuteTimedCommand(new CreateCategoryCommand(_categoryService, CategoryType.Expense, ReadLine("Название"))));
        expenseCategoryMenu.AddSeparator();
        expenseCategoryMenu.AddItem("   → Назад", Menu.GoBackAction);

        // Подменю: Пересчёт баланса
        var recalcMenu = new Menu("Пересчёт баланса");
        recalcMenu.AddItem("1. Автоматический (по операциям)", () => ExecuteTimedCommand(new RecalculateBalancesCommand(_analyticsService)));
        recalcMenu.AddItem("2. Установить вручную", SetBalanceManually);
        recalcMenu.AddSeparator();
        recalcMenu.AddItem("   → Назад", Menu.GoBackAction);

        // Подменю: Детальная статистика
        var statsMenu = new Menu("Детальная статистика");
        statsMenu.AddItem("1. По каждому счёту", ShowPerAccountStatistics);
        statsMenu.AddItem("2. Общая по всем счетам", ShowOverallStatistics);
        statsMenu.AddItem("3. Анализ по категориям", ShowDetailedCategoryAnalysis);
        statsMenu.AddItem("4. Доходы vs Расходы (по счётам)", ShowIncomeVsExpensesByAccount);
        statsMenu.AddSeparator();
        statsMenu.AddItem("   → Назад", Menu.GoBackAction);

        // Главное меню
        var mainMenu = new Menu("ВШЭ-Банк : Учет финансов", isMainMenu: true);
        Func<bool> hasAccounts = () => _accountService.GetAll().Any();
        Func<bool> hasOperations = () => _operationService.GetAll().Any();

        mainMenu.AddItem("1. Создать счёт →", null, createAccountMenu);
        mainMenu.AddItem("2. Категории доходов →", null, incomeCategoryMenu);
        mainMenu.AddItem("3. Категории расходов →", null, expenseCategoryMenu);
        mainMenu.AddItem("4. Добавить операцию", () => ExecuteTimedCommand(new AddOperationCommand(_accountService, _categoryService, _operationService, _analyticsService)));
        mainMenu.AddSeparator();
        mainMenu.AddItem("5. Доходы vs Расходы", ShowIncomeVsExpenses, isEnabled: hasOperations);
        mainMenu.AddItem("6. Детальная статистика →", null, statsMenu, isEnabled: hasOperations);
        mainMenu.AddItem("7. Пересчёт баланса →", null, recalcMenu, isEnabled: hasAccounts);
        mainMenu.AddSeparator();
        mainMenu.AddItem("8. Экспорт в JSON", () => ExecuteTimedCommand(new ExportDataCommand(_fileService)));
        mainMenu.AddItem("9. Импорт из JSON", () => ExecuteTimedCommand(new ImportDataCommand(_fileService, _analyticsService)));
        mainMenu.AddSeparator();
        mainMenu.AddItem("0. Выход", Menu.ExitProgram);

        mainMenu.Display();
    }

    /// <summary>
    /// Выполняет предзагрузку тестовых данных для демонстрации.
    /// </summary>
    public void PrepopulateData()
    {
        Console.Clear();
        DrawBoxedHeader("Авто-загрузка тестовых данных", 80);
        try
        {
            var acc = _accountService.CreateAccount("Основной счёт");
            var incCat = _categoryService.CreateCategory(CategoryType.Income, "Зарплата");
            var expCat = _categoryService.CreateCategory(CategoryType.Expense, "Кафе");

            _operationService.AddOperation(OperationType.Income, acc.Id, 50000m, DateTime.Now, "Оклад", incCat.Id);
            _operationService.AddOperation(OperationType.Expense, acc.Id, -1200m, DateTime.Now, "Обед", expCat.Id);

            ShowSuccess("Тестовые данные загружены. Баланс пересчитан.");
            _analyticsService.RecalculateBalances();
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка загрузки: {ex.Message}");
        }
        Console.WriteLine("\nНажмите любую клавишу...");
        Console.ReadKey();
    }

    #region Команды для измерения времени (паттерн Команда + Декоратор)

    /// <summary>
    /// Выполняет команду с измерением времени её выполнения.
    /// </summary>
    private void ExecuteTimedCommand<T>(ICommand<T> command)
    {
        var timed = new TimedCommandDecorator<T>(command, msg =>
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"⏱️ {msg}");
            Console.ResetColor();
        });
        timed.Execute();
    }

    // === Команда: Создание счёта ===
    private class CreateAccountCommand : ICommand<bool>
    {
        private readonly IAccountService _service;
        private readonly string _name;
        public CreateAccountCommand(IAccountService service, string name) { _service = service; _name = name; }
        public bool Execute()
        {
            if (string.IsNullOrWhiteSpace(_name)) { ShowError("Название обязательно."); return false; }
            try
            {
                var acc = _service.CreateAccount(_name);
                ShowSuccess($"Счёт '{acc.Name}' создан. Баланс: {acc.Balance:C}");
                return true;
            }
            catch (Exception ex) { ShowError(ex.Message); return false; }
        }
    }

    // === Команда: Создание категории ===
    private class CreateCategoryCommand : ICommand<bool>
    {
        private readonly ICategoryService _service;
        private readonly CategoryType _type;
        private readonly string _name;
        public CreateCategoryCommand(ICategoryService service, CategoryType type, string name) { _service = service; _type = type; _name = name; }
        public bool Execute()
        {
            if (string.IsNullOrWhiteSpace(_name)) return false;
            try
            {
                var cat = _service.CreateCategory(_type, _name);
                ShowSuccess($"Категория '{cat.Name}' ({_type}) создана.");
                return true;
            }
            catch (Exception ex) { ShowError(ex.Message); return false; }
        }
    }

    // === Команда: Автоматический пересчёт баланса ===
    private class RecalculateBalancesCommand : ICommand<bool>
    {
        private readonly IAnalyticsService _service;
        public RecalculateBalancesCommand(IAnalyticsService service) { _service = service; }
        public bool Execute()
        {
            try
            {
                _service.RecalculateBalances();
                ShowSuccess("Балансы всех счетов пересчитаны!");
                return true;
            }
            catch (Exception ex) { ShowError(ex.Message); return false; }
        }
    }

    // === Команда: Экспорт в JSON ===
    private class ExportDataCommand : ICommand<bool>
    {
        private readonly IFileImportExportService _service;

        public ExportDataCommand(IFileImportExportService service)
        {
            _service = service;
        }

        public bool Execute()
        {
            Console.Clear();
            const int width = 80;

            DrawBoxedHeader("Экспорт данных в JSON", width);

            Console.Write("║ Введите путь (enter = 'finance.json'): ");
            string path = Console.ReadLine() ?? "";
            
            Console.WriteLine("╚" + new string('═', width - 2) + "╝");

            if (string.IsNullOrWhiteSpace(path))
            {
                path = "finance.json";
            }

            try
            {
                _service.ExportToJson(path);
                ShowSuccess($"Экспорт в '{path}' завершён.");
                return true;
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка экспорта: {ex.Message}");
                return false;
            }
        }
    }

    // === Команда: Импорт из JSON ===
    private class ImportDataCommand : ICommand<bool>
    {
        private readonly IFileImportExportService _service;
        private readonly IAnalyticsService _analytics;

        public ImportDataCommand(IFileImportExportService service, IAnalyticsService analytics)
        {
            _service = service;
            _analytics = analytics;
        }

        public bool Execute()
        {
            Console.Clear();
            const int width = 80;
            
            DrawBoxedHeader("Импорт данных из JSON", width);
            
            Console.Write("║ Введите путь к файлу для импорта: ");
            string path = Console.ReadLine() ?? "";
            
            Console.WriteLine("╚" + new string('═', width - 2) + "╝");

            if (string.IsNullOrWhiteSpace(path))
            {
                ShowError("Путь не указан.");
                return false;
            }

            if (!File.Exists(path))
            {
                ShowError($"Файл не найден: {path}");
                return false;
            }

            try
            {
                _service.ImportFromJson(path);
                _analytics.RecalculateBalances();
                ShowSuccess($"Импорт из '{path}' завершён. \nБалансы пересчитаны.");
                return true;
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка импорта: {ex.Message}");
                return false;
            }
        }
    }

    // === Команда: Добавление операции ===
    private class AddOperationCommand : ICommand<bool>
    {
        private readonly IAccountService _accService;
        private readonly ICategoryService _catService;
        private readonly IOperationService _opService;
        private readonly IAnalyticsService _analytics;

        public AddOperationCommand(
            IAccountService accService,
            ICategoryService catService,
            IOperationService opService,
            IAnalyticsService analytics)
        {
            _accService = accService;
            _catService = catService;
            _opService = opService;
            _analytics = analytics;
        }

        public bool Execute()
        {
            var accounts = _accService.GetAll().ToList();
            var categories = _catService.GetAll().ToList();
            if (!accounts.Any() || !categories.Any()) { ShowError("Нет счетов или категорий."); return false; }

            Console.Clear();
            DrawBoxedHeader("Добавление новой операции", 80);
            Console.WriteLine();

            // Выбор счёта
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Доступные счета:");
            Console.ResetColor();
            for (int i = 0; i < accounts.Count; i++)
                Console.WriteLine($"  {i + 1}. {accounts[i].Name} ({accounts[i].Balance:C})");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Номер счёта: ");
            Console.ResetColor();
            string? accountInput = Console.ReadLine();
            if (!int.TryParse(accountInput, out int aIdx) || aIdx < 1 || aIdx > accounts.Count)
            { ShowError("Неверный номер счёта."); return false; }
            var acc = accounts[aIdx - 1];

            // Выбор категории
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nДоступные категории:");
            Console.ResetColor();
            for (int i = 0; i < categories.Count; i++)
                Console.WriteLine($"  {i + 1}. {categories[i].Type} — {categories[i].Name}");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Номер категории: ");
            Console.ResetColor();
            string? categoryInput = Console.ReadLine();
            if (!int.TryParse(categoryInput, out int cIdx) || cIdx < 1 || cIdx > categories.Count)
            { ShowError("Неверный номер категории."); return false; }
            var cat = categories[cIdx - 1];

            // Определение типа операции
            var opType = cat.Type == CategoryType.Income ? OperationType.Income : OperationType.Expense;

            // Ввод суммы
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"Сумма ({opType}): ");
            Console.ResetColor();
            string? amountInput = Console.ReadLine();
            if (!decimal.TryParse(amountInput, out decimal amount))
            { ShowError("Неверная сумма."); return false; }
            if (opType == OperationType.Expense && amount > 0) amount = -amount;
            if (opType == OperationType.Income && amount < 0) amount = -amount;

            // Ввод описания
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("Описание (опционально): ");
            Console.ResetColor();
            string? desc = Console.ReadLine();

            // Выполнение операции
            try
            {
                _opService.AddOperation(opType, acc.Id, amount, DateTime.Now, desc, cat.Id);
                ShowSuccess($"Операция добавлена: {amount:C} → {acc.Name}");
                _analytics.RecalculateBalances();
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                return false;
            }
        }
    }

    #endregion

    #region Методы аналитики

    /// <summary>
    /// Отображает меню для ручного пересчёта баланса.
    /// </summary>
    private void SetBalanceManually()
    {
        var accounts = _accountService.GetAll().ToList();
        if (!accounts.Any()) { ShowError("Нет счетов."); return; }

        var menu = new Menu("Выберите счёт для ручного пересчёта");
        foreach (var acc in accounts)
        {
            menu.AddItem($"   → {acc.Name} ({acc.Balance:C})", () =>
            {
                Console.Write($"Новый баланс для '{acc.Name}': ");
                string? newBalStr = Console.ReadLine();
                if (!decimal.TryParse(newBalStr, out decimal newBal))
                { ShowError("Неверный формат суммы."); return; }
                try
                {
                    _accountService.SetBalanceManually(acc.Id, newBal);
                    ShowSuccess($"Баланс обновлён: {newBal:C}");
                }
                catch (Exception ex) { ShowError(ex.Message); }
            });
        }
        menu.AddSeparator();
        menu.AddItem("   → Отмена", Menu.GoBackAction);
        menu.Display();
    }

    /// <summary>
    /// Отображает разницу между доходами и расходами.
    /// </summary>
    private void ShowIncomeVsExpenses()
    {
        var res = _analyticsService.GetIncomeVsExpenses(DateTime.MinValue, DateTime.MaxValue);
        DrawBoxedContent("Доходы vs Расходы", $"Итоговая разница: {res:C}", res >= 0 ? ConsoleColor.Green : ConsoleColor.Red);
    }

    /// <summary>
    /// Отображает меню выбора счёта и типа анализа (агрегированный или детализированный).
    /// </summary>
    private void ShowPerAccountStatistics()
    {
        var accounts = _accountService.GetAll().ToList();
        if (!accounts.Any())
        {
            ShowError("Нет счетов для анализа.");
            return;
        }

        var accountMenu = new Menu("Выберите счёт для анализа");
        foreach (var acc in accounts)
        {
            accountMenu.AddItem($"   → {acc.Name}", () =>
            {
                var analysisMenu = new Menu($"Анализ счёта: {acc.Name}");
                analysisMenu.AddItem("1. Общая статистика", () => ShowAccountSummary(acc));
                analysisMenu.AddItem("2. Детализированный отчёт по операциям", () => ShowAccountOperations(acc));
                analysisMenu.AddSeparator();
                analysisMenu.AddItem("   → Назад", Menu.GoBackAction);
                analysisMenu.Display();
            });
        }
        accountMenu.AddSeparator();
        accountMenu.AddItem("   → Назад", Menu.GoBackAction);
        accountMenu.Display();
    }

    /// <summary>
    /// Отображает агрегированную статистику по счёту.
    /// </summary>
    private void ShowAccountSummary(BankAccount account)
    {
        var operations = _operationService.GetAll()
            .Where(o => o.BankAccountId == account.Id)
            .ToList();

        var income = operations.Where(o => o.Type == OperationType.Income).Sum(o => o.Amount);
        var expense = operations.Where(o => o.Type == OperationType.Expense).Sum(o => o.Amount);
        var net = income + expense;

        var sb = new StringBuilder();
        sb.AppendLine($"СЧЁТ: {account.Name}".PadRight(76));
        sb.AppendLine(new string('=', 76));
        sb.AppendLine($"Текущий баланс:      {account.Balance:C}");
        sb.AppendLine($"Общий доход:         {income:C}");
        sb.AppendLine($"Общий расход:        {Math.Abs(expense):C}");
        sb.AppendLine($"Чистая прибыль:      {net:C}");
        sb.AppendLine(new string('-', 76));

        DrawBoxedContent("Общая статистика", sb.ToString(), ConsoleColor.Cyan);
    }

    /// <summary>
    /// Отображает детализированный отчёт по операциям счёта.
    /// </summary>
    private void ShowAccountOperations(BankAccount account)
    {
        var operations = _operationService.GetAll()
            .Where(o => o.BankAccountId == account.Id)
            .OrderByDescending(o => o.Date)
            .ToList();

        if (!operations.Any())
        {
            ShowError("По этому счёту ещё нет операций.");
            return;
        }

        var categories = _categoryService.GetAll()
            .ToDictionary(c => c.Id, c => c.Name);

        var sb = new StringBuilder();
        sb.AppendLine($"ДЕТАЛИЗАЦИЯ СЧЁТА: {account.Name}".PadRight(76));
        sb.AppendLine(new string('=', 76));

        foreach (var op in operations)
        {
            string categoryName = categories.TryGetValue(op.CategoryId, out var name) ? name : "Неизвестная";
            string typeIndicator = op.Type == OperationType.Income ? "+" : "-";
            string amountStr = $"{typeIndicator}{Math.Abs(op.Amount):C}";

            sb.AppendLine($"Дата:      {op.Date:dd.MM.yyyy HH:mm}");
            sb.AppendLine($"Категория: {categoryName}");
            sb.AppendLine($"Сумма:     {amountStr}");
            if (!string.IsNullOrEmpty(op.Description))
                sb.AppendLine($"Описание:  {op.Description}");
            sb.AppendLine(new string('-', 76));
        }

        DrawBoxedContent("Детализированный отчёт", sb.ToString(), ConsoleColor.Magenta);
    }

    /// <summary>
    /// Отображает анализ по категориям.
    /// </summary>
    private void ShowDetailedCategoryAnalysis()
    {
        var analysis = _analyticsService.GetDetailedCategoryAnalysis();
        var sb = new StringBuilder();
        sb.AppendLine("АНАЛИЗ ПО КАТЕГОРИЯМ".PadRight(76));
        sb.AppendLine(new string('=', 76));
        foreach (var (name, summary) in analysis.OrderBy(x => x.Key))
        {
            sb.AppendLine($"Категория: {name} ({summary.CategoryType})");
            sb.AppendLine($"  ├─ Операций: {summary.OperationCount}");
            sb.AppendLine($"  ├─ Доход:    {summary.TotalIncome:C}");
            sb.AppendLine($"  ├─ Расход:   {Math.Abs(summary.TotalExpense):C}");
            sb.AppendLine($"  └─ Средний чек: {summary.AverageAmount:C}");
            sb.AppendLine(new string('-', 76));
        }
        DrawBoxedContent("Анализ по категориям", sb.ToString(), ConsoleColor.Magenta);
    }

    /// <summary>
    /// Отображает общую статистику по всем счетам.
    /// </summary>
    private void ShowOverallStatistics()
    {
        var stats = _analyticsService.GetOverallStatistics();
        var sb = new StringBuilder();
        sb.AppendLine("ОБЩАЯ СТАТИСТИКА".PadRight(76));
        sb.AppendLine(new string('=', 76));
        sb.AppendLine($"Всего счетов: {stats.TotalAccounts}");
        sb.AppendLine($"Общий баланс: {stats.TotalBalance:C}");
        sb.AppendLine($"Всего операций: {stats.TotalOperations}");
        sb.AppendLine(new string('-', 76));
        sb.AppendLine($"Общий доход: {stats.TotalIncome:C}");
        sb.AppendLine($"Общий расход: {Math.Abs(stats.TotalExpense):C}");
        sb.AppendLine($"Чистая прибыль: {stats.NetProfit:C}");
        DrawBoxedContent("Общая статистика", sb.ToString(), ConsoleColor.Green);
    }

    /// <summary>
    /// Отображает разницу доходов и расходов по каждому счёту и в целом.
    /// </summary>
    private void ShowIncomeVsExpensesByAccount()
    {
        var accounts = _accountService.GetAll().ToList();
        var operations = _operationService.GetAll().ToList();

        if (!accounts.Any() || !operations.Any())
        {
            ShowError("Недостаточно данных для анализа.");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("ДОХОДЫ vs РАСХОДЫ ПО СЧЁТАМ".PadRight(76));
        sb.AppendLine(new string('=', 76));

        decimal totalIncomeAll = 0;
        decimal totalExpenseAll = 0;

        foreach (var acc in accounts)
        {
            var accOps = operations.Where(o => o.BankAccountId == acc.Id).ToList();
            var income = accOps.Where(o => o.Type == OperationType.Income).Sum(o => o.Amount);
            var expense = accOps.Where(o => o.Type == OperationType.Expense).Sum(o => o.Amount);
            var net = income + expense;

            totalIncomeAll += income;
            totalExpenseAll += expense;

            sb.AppendLine($"Счёт: {acc.Name}");
            sb.AppendLine($"  ├─ Доходы:   {income:C}");
            sb.AppendLine($"  ├─ Расходы:  {Math.Abs(expense):C}");
            sb.AppendLine($"  └─ Разница:  {net:C} {(net >= 0 ? "(профицит)" : "(дефицит)")}");
            sb.AppendLine(new string('-', 76));
        }

        // Итог по всем счетам
        var totalNet = totalIncomeAll + totalExpenseAll;
        sb.AppendLine($"ИТОГО ПО ВСЕМ СЧЁТАМ".PadRight(76));
        sb.AppendLine(new string('-', 76));
        sb.AppendLine($"Всего доходов: {totalIncomeAll:C}");
        sb.AppendLine($"Всего расходов: {Math.Abs(totalExpenseAll):C}");
        sb.AppendLine($"Чистая разница: {totalNet:C} {(totalNet >= 0 ? "(профицит)" : "(дефицит)")}");

        DrawBoxedContent("Доходы vs Расходы по счётам", sb.ToString(), totalNet >= 0 ? ConsoleColor.Green : ConsoleColor.Red);
    }

    #endregion

    #region Вспомогательные методы

    /// <summary>
    /// Выводит приглашение ко вводу и возвращает введённую строку.
    /// </summary>
    private static string ReadLine(string prompt)
    {
        Console.Write($"{prompt}: ");
        return Console.ReadLine() ?? "";
    }

    /// <summary>
    /// Выводит сообщение об ошибке красным цветом.
    /// </summary>
    private static void ShowError(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[ОШИБКА] {msg}");
        Console.ResetColor();
    }

    /// <summary>
    /// Выводит сообщение об успехе зелёным цветом.
    /// </summary>
    private static void ShowSuccess(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n[УСПЕХ] {msg}");
        Console.ResetColor();
    }

    /// <summary>
    /// Рисует рамку с заголовком.
    /// </summary>
    public static void DrawBoxedHeader(string title, int width = 60)
    {
        Console.WriteLine("╔" + new string('═', width - 2) + "╗");
        var line = "║" + PadCenter(title, width - 2) + "║";

        // Разделяем строку, чтобы покрасить только заголовок
        int titleStartIndex = line.IndexOf(title, StringComparison.Ordinal);
        Console.Write(line.Substring(0, titleStartIndex));
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.Write(title);
        Console.ResetColor();
        Console.WriteLine(line.Substring(titleStartIndex + title.Length));
        
        Console.WriteLine("╠" + new string('═', width - 2) + "╣");
    }

    /// <summary>
    /// Рисует содержимое в рамке с указанным цветом.
    /// </summary>
    private static void DrawBoxedContent(string title, string content, ConsoleColor color)
    {
        const int w = 80;
        DrawBoxedHeader(title, w);
        foreach (var line in content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            Console.Write("║ ");
            Console.ForegroundColor = color;
            Console.Write(line.PadRight(w - 4));
            Console.ResetColor();
            Console.WriteLine(" ║");
        }
        Console.WriteLine("╚" + new string('═', w - 2) + "╝");
    }

    /// <summary>
    /// Центрирует текст в строке заданной ширины.
    /// </summary>
    private static string PadCenter(string s, int w)
    {
        if (string.IsNullOrEmpty(s)) return "".PadRight(w);
        if (s.Length >= w) return s.Substring(0, w);
        int totalPadding = w - s.Length;
        int padLeft = totalPadding / 2 + s.Length;
        return s.PadLeft(padLeft).PadRight(w);
    }

    #endregion
}