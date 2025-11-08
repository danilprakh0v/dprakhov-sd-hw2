/*
 * Проект: FinanceTracker
 * Файл: TimedCommandDecorator.cs
 * Расположение: FinanceTracker.Application/Decorators/
 * Назначение: Декоратор для измерения времени выполнения команды. 
  * ==============================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
namespace FinanceTracker.Application.Decorators;

using System;
using System.Diagnostics;
using FinanceTracker.Application.Commands;

/// <summary>
/// Декоратор (GoF Decorator) для команд, измеряющий время их выполнения.
/// Оборачивает любую команду, реализующую <see cref="ICommand{TResult}"/>,
/// и логирует длительность её выполнения через переданный логгер.
/// </summary>
/// <typeparam name="TResult"> Тип результата команды. </typeparam>
public class TimedCommandDecorator<TResult> : ICommand<TResult>
{
    private readonly ICommand<TResult> _command;
    private readonly Action<string> _logger;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="TimedCommandDecorator{TResult}"/>.
    /// </summary>
    /// <param name="command"> Команда, время выполнения которой необходимо измерить. </param>
    /// <param name="logger"> Действие для вывода лога (например, Console.WriteLine). </param>
    /// <exception cref="ArgumentNullException"> Выбрасывается, если command или logger равны null. </exception>
    public TimedCommandDecorator(ICommand<TResult> command, Action<string> logger)
    {
        _command = command ?? throw new ArgumentNullException(nameof(command));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Выполняет команду, измеряет время её выполнения и логирует результат.
    /// </summary>
    /// <returns> Результат выполнения оригинальной команды. </returns>
    public TResult Execute()
    {
        var stopwatch = Stopwatch.StartNew();
        var result = _command.Execute();
        stopwatch.Stop();

        _logger($"Команда завершена за {stopwatch.ElapsedMilliseconds} мс. \nТип результата: {typeof(TResult).Name}");
        return result;
    }
}