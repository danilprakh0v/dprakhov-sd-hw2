/*
 * Проект: FinanceTracker
 * Файл: EntityTests.cs
 * Расположение: FinanceTracker.Tests/Core/
 * Назначение: Юнит-тесты для базовых сущностей проекта.
 * ======================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
using Xunit;
using FinanceTracker.Core.Entities;
using System;

namespace FinanceTracker.Tests.Core;

/// <summary>
/// Тесты для проверки корректной инициализации основных сущностей.
/// </summary>
public class EntityTests
{
    /// <summary>
    /// Проверяет, что конструктор BankAccount корректно устанавливает имя и баланс по умолчанию.
    /// </summary>
    [Fact]
    public void BankAccount_Constructor_ShouldSetNameAndDefaultBalance()
    {
        // Arrange
        var name = "Тестовый счёт";

        // Act
        var account = new BankAccount(name);

        // Assert
        Assert.Equal(name, account.Name);
        Assert.Equal(0m, account.Balance);
        Assert.NotEqual(Guid.Empty, account.Id);
    }

    /// <summary>
    /// Проверяет, что конструктор Category корректно устанавливает имя и тип.
    /// </summary>
    [Fact]
    public void Category_Constructor_ShouldSetNameAndType()
    {
        // Arrange
        var name = "Продукты";
        var type = CategoryType.Expense;

        // Act
        var category = new Category(type, name);

        // Assert
        Assert.Equal(name, category.Name);
        Assert.Equal(type, category.Type);
        Assert.NotEqual(Guid.Empty, category.Id);
    }
    
    /// <summary>
    /// Проверяет, что конструктор Operation корректно устанавливает все свойства.
    /// </summary>
    [Fact]
    public void Operation_Constructor_ShouldSetAllProperties()
    {
        // Arrange
        var type = OperationType.Income;
        var accountId = Guid.NewGuid();
        var amount = 1500m;
        var date = DateTime.Now;
        var description = "Аванс";
        var categoryId = Guid.NewGuid();

        // Act
        var operation = new Operation(type, accountId, amount, date, description, categoryId);

        // Assert
        Assert.Equal(type, operation.Type);
        Assert.Equal(accountId, operation.BankAccountId);
        Assert.Equal(amount, operation.Amount);
        Assert.Equal(date, operation.Date);
        Assert.Equal(description, operation.Description);
        Assert.Equal(categoryId, operation.CategoryId);
    }
}