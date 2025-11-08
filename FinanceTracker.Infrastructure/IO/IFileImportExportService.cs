/*
 * Проект: FinanceTracker
 * Файл: IFileImportExportService.cs
 * Расположение: FinanceTracker.Infrastructure/IO/
 * Назначение: Интерфейс сервиса импорта и экспорта данных в формате JSON.
 * =======================================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
namespace FinanceTracker.Infrastructure.IO;

/// <summary>
/// Интерфейс сервиса для импорта и экспорта всей финансовой модели в/из JSON-файла.
/// </summary>
public interface IFileImportExportService
{
    /// <summary>
    /// Импортирует данные из JSON-файла в текущее хранилище.
    /// </summary>
    /// <param name="filePath">Путь к JSON-файлу.</param>
    /// <exception cref="System.IO.FileNotFoundException"> Выбрасывается, если файл не найден. </exception>
    /// <exception cref="System.Text.Json.JsonException"> Выбрасывается при ошибке десериализации. </exception>
    void ImportFromJson(string filePath);

    /// <summary>
    /// Экспортирует все данные из текущего хранилища в JSON-файл.
    /// </summary>
    /// <param name="filePath"> Путь к выходному JSON-файлу. </param>
    void ExportToJson(string filePath);
}