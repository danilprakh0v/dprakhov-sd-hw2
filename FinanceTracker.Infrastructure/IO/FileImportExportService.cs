/*
 * Проект: FinanceTracker
 * Файл: FileImportExportService.cs
 * Расположение: FinanceTracker.Infrastructure/IO/
 * Назначение: Реализация сервиса импорта и экспорта данных через JSON-обработчик.
 * ===============================================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
namespace FinanceTracker.Infrastructure.IO;

using FinanceTracker.Core.Interfaces;

/// <summary>
/// Реализация сервиса импорта и экспорта данных в JSON.
/// Служит фасадом над JsonHandler, скрывая детали парсинга от вышестоящих слоёв.
/// </summary>
public class FileImportExportService : IFileImportExportService
{
    private readonly IBankAccountRepository _accountRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IOperationRepository _operationRepo;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="FileImportExportService"/>.
    /// </summary>
    /// <param name="accountRepo"> Репозиторий счетов. </param>
    /// <param name="categoryRepo"> Репозиторий категорий. </param>
    /// <param name="operationRepo"> Репозиторий операций. </param>
    public FileImportExportService(
        IBankAccountRepository accountRepo,
        ICategoryRepository categoryRepo,
        IOperationRepository operationRepo)
    {
        _accountRepo = accountRepo;
        _categoryRepo = categoryRepo;
        _operationRepo = operationRepo;
    }

    /// <inheritdoc />
    public void ImportFromJson(string filePath)
    {
        var handler = new JsonHandler(_accountRepo, _categoryRepo, _operationRepo);
        handler.Import(filePath);
    }

    /// <inheritdoc />
    public void ExportToJson(string filePath)
    {
        var handler = new JsonHandler(_accountRepo, _categoryRepo, _operationRepo);
        handler.Export(filePath);
    }
}