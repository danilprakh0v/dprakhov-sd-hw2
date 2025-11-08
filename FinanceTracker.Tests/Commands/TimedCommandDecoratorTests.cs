/*
 * Проект: FinanceTracker
 * Файл: TimedCommandDecoratorTests.cs
 * Расположение: FinanceTracker.Tests/Commands/
 * Назначение: Юнит-тесты для декоратора измерения времени.
 * ===========================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
using Moq;
using Xunit;
using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Decorators;
using System;

namespace FinanceTracker.Tests.Commands;

/// <summary>
/// Тесты для декоратора, измеряющего время выполнения команды.
/// </summary>
public class TimedCommandDecoratorTests
{
    /// <summary>
    /// Проверяет, что декоратор вызывает внутреннюю команду, возвращает её результат и вызывает логгер.
    /// </summary>
    [Fact]
    public void Execute_WhenCalled_ShouldWrapCommandAndLogExecutionTime()
    {
        // Arrange
        var commandExecuted = false;
        var loggerCalled = false;
        var mockCommand = new Mock<ICommand<string>>();
        
        mockCommand.Setup(c => c.Execute())
            .Callback(() => commandExecuted = true)
            .Returns("result");

        var decorator = new TimedCommandDecorator<string>(
            mockCommand.Object,
            msg => loggerCalled = true
        );

        // Act
        var result = decorator.Execute();

        // Assert
        Assert.True(commandExecuted, "Внутренняя команда не была вызвана.");
        Assert.True(loggerCalled, "Логгер не был вызван.");
        Assert.Equal("result", result);
    }
}