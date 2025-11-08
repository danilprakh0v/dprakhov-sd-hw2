/*
 * Проект: FinanceTracker
 * Файл: AnalyticsServiceTests.cs
 * Расположение: FinanceTracker.Tests/Services/
 * Назначение: Юнит-тесты для AnalyticsService — проверка всей аналитики и пересчёта баланса.
 * ==========================================================================================
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
using System;
using System.Linq;

namespace FinanceTracker.Tests.Services;

/// <summary>
/// Тесты для сервиса финансовой аналитики.
/// </summary>
public class AnalyticsServiceTests
{
    private readonly Mock<IOperationRepository> _mockOpRepo;
    private readonly Mock<ICategoryRepository> _mockCatRepo;
    private readonly Mock<IBankAccountRepository> _mockAccRepo;
    private readonly AnalyticsService _service;

    public AnalyticsServiceTests()
    {
        _mockOpRepo = new Mock<IOperationRepository>();
        _mockCatRepo = new Mock<ICategoryRepository>();
        _mockAccRepo = new Mock<IBankAccountRepository>();
        _service = new AnalyticsService(_mockOpRepo.Object, _mockCatRepo.Object, _mockAccRepo.Object);
    }

    /// <summary>
    /// Проверяет корректность расчёта разницы доходов и расходов.
    /// </summary>
    [Fact]
    public void GetIncomeVsExpenses_WithOperations_ShouldReturnCorrectDifference()
    {
        // Arrange
        var operations = new[]
        {
            new Operation(OperationType.Income, Guid.NewGuid(), 10000m, DateTime.Now, null, Guid.NewGuid()),
            new Operation(OperationType.Expense, Guid.NewGuid(), -3000m, DateTime.Now, null, Guid.NewGuid())
        };
        _mockOpRepo.Setup(r => r.GetAll()).Returns(operations);

        // Act
        var result = _service.GetIncomeVsExpenses(DateTime.MinValue, DateTime.MaxValue);

        // Assert
        Assert.Equal(7000m, result);
    }

    /// <summary>
    /// Проверяет, что при отсутствии операций разница доходов и расходов равна нулю.
    /// </summary>
    [Fact]
    public void GetIncomeVsExpenses_WithNoOperations_ShouldReturnZero()
    {
        // Arrange
        _mockOpRepo.Setup(r => r.GetAll()).Returns(Enumerable.Empty<Operation>());
        
        // Act
        var result = _service.GetIncomeVsExpenses(DateTime.MinValue, DateTime.MaxValue);

        // Assert
        Assert.Equal(0m, result);
    }

    /// <summary>
    /// Проверяет, что балансы счетов корректно пересчитываются на основе операций.
    /// </summary>
    [Fact]
    public void RecalculateBalances_WithOperations_ShouldCorrectlyUpdateAccountBalances()
    {
        // Arrange
        var account = new BankAccount("Test"); 
        var operations = new[]
        {
            new Operation(OperationType.Income, account.Id, 5000m, DateTime.Now, null, Guid.NewGuid()),
            new Operation(OperationType.Expense, account.Id, -1500m, DateTime.Now, null, Guid.NewGuid())
        };
        _mockAccRepo.Setup(r => r.GetAll()).Returns(new[] { account });
        _mockOpRepo.Setup(r => r.GetAll()).Returns(operations);

        // Act
        _service.RecalculateBalances();

        // Assert
        Assert.Equal(3500m, account.Balance);
        _mockAccRepo.Verify(r => r.Update(account), Times.Once);
    }

    /// <summary>
    /// Проверяет, что общая статистика по всем счетам и операциям считается верно.
    /// </summary>
    [Fact]
    public void GetOverallStatistics_WithData_ShouldReturnCorrectStatistics()
    {
        // Arrange
        var acc1 = new BankAccount("Счёт 1") { Balance = 3500m };
        var acc2 = new BankAccount("Счёт 2") { Balance = 1000m };
        var operations = new[]
        {
            new Operation(OperationType.Income, acc1.Id, 5000m, DateTime.Now, null, Guid.NewGuid()),
            new Operation(OperationType.Expense, acc1.Id, -1500m, DateTime.Now, null, Guid.NewGuid()),
            new Operation(OperationType.Income, acc2.Id, 1000m, DateTime.Now, null, Guid.NewGuid()),
        };
        _mockAccRepo.Setup(r => r.GetAll()).Returns(new[] { acc1, acc2 });
        _mockOpRepo.Setup(r => r.GetAll()).Returns(operations);

        // Act
        var stats = _service.GetOverallStatistics();

        // Assert
        Assert.Equal(2, stats.TotalAccounts);
        Assert.Equal(4500m, stats.TotalBalance);
        Assert.Equal(3, stats.TotalOperations);
        Assert.Equal(6000m, stats.TotalIncome);
        Assert.Equal(-1500m, stats.TotalExpense);
    }
    
    /// <summary>
    /// Проверяет, что детальный анализ по категориям формируется корректно.
    /// </summary>
    [Fact]
    public void GetDetailedCategoryAnalysis_WithData_ShouldReturnCorrectAnalysis()
    {
        // Arrange
        var catFood = new Category(CategoryType.Expense, "Еда");
        var catSalary = new Category(CategoryType.Income, "Зарплата");
        var operations = new[]
        {
            new Operation(OperationType.Income, Guid.NewGuid(), 50000m, DateTime.Now, null, catSalary.Id),
            new Operation(OperationType.Expense, Guid.NewGuid(), -1200m, DateTime.Now, null, catFood.Id),
            new Operation(OperationType.Expense, Guid.NewGuid(), -800m, DateTime.Now, null, catFood.Id)
        };
        _mockCatRepo.Setup(r => r.GetAll()).Returns(new[] { catFood, catSalary });
        _mockOpRepo.Setup(r => r.GetAll()).Returns(operations);
        
        // Act
        var analysis = _service.GetDetailedCategoryAnalysis();
        
        // Assert
        Assert.Equal(2, analysis.Count);
        Assert.Equal(50000m, analysis["Зарплата"].TotalIncome);
        Assert.Equal(-2000m, analysis["Еда"].TotalExpense);
        Assert.Equal(2, analysis["Еда"].OperationCount);
    }
}