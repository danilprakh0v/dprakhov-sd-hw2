/*
 * Проект: FinanceTracker
 * Файл: ICommand.cs
 * Расположение: FinanceTracker.Application/Commands/
 * Назначение: Обобщённый интерфейс паттерна Команда - (GoF Command). 
 * ==================================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
namespace FinanceTracker.Application.Commands;

/// <summary>
/// Интерфейс паттерна Команда.
/// </summary>
/// <typeparam name="TResult"> Тип результата выполнения команды. </typeparam>
public interface ICommand<out TResult>
{
    /// <summary>
    /// Выполняет инкапсулированный сценарий и возвращает результат.
    /// </summary>
    /// <returns> Результат выполнения команды. </returns>
    TResult Execute();
}