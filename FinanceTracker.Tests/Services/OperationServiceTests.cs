/*
 * Проект: FinanceTracker
 * Файл: OperationServiceTests.cs
 * Расположение: FinanceTracker.Tests/Services/
 * Назначение: Юнит-тесты для OperationService — проверка валидации и добавления операций.
 * =======================================================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
using Moq;
using Xunit;
using FinanceTracker.Application.Services;
using FinanceTracker.Core.Entities;
using FinanceTracker.Core.Interfaces;

namespace FinanceTracker.Tests.Services;

public class OperationServiceTests
{
    [Fact]
    public void AddOperation_ValidParameters_AddsOperation()
    {
        // Arrange
        var mockRepo = new Mock<IOperationRepository>();
        var service = new OperationService(mockRepo.Object);
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        // Act
        var operation = service.AddOperation(
            OperationType.Income,
            accountId,
            5000m,
            DateTime.Now,
            "Зарплата",
            categoryId
        );

        // Assert
        Assert.NotNull(operation);
        Assert.Equal(OperationType.Income, operation.Type);
        Assert.Equal(5000m, operation.Amount);
        mockRepo.Verify(r => r.Add(It.IsAny<Operation>()), Times.Once);
    }

    [Fact]
    public void AddOperation_IncomeWithNegativeAmount_ThrowsArgumentException()
    {
        var mockRepo = new Mock<IOperationRepository>();
        var service = new OperationService(mockRepo.Object);
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() =>
            service.AddOperation(OperationType.Income, accountId, -100m, DateTime.Now, null, categoryId));
    }
}