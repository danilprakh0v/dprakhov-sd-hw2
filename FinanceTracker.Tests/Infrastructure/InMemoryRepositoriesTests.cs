/*
 * Проект: FinanceTracker
 * Файл: InMemoryRepositoryTests.cs
 * Расположение: FinanceTracker.Tests/Infrastructure/
 * Назначение: Интеграционные тесты для InMemory репозиториев.
 * ===========================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
using Xunit;
using FinanceTracker.Core.Entities;
using FinanceTracker.Infrastructure.Repositories;
using System;
using System.Linq;

namespace FinanceTracker.Tests.Infrastructure;

/// <summary>
/// Тесты для реализаций репозиториев, работающих в памяти.
/// Эти тесты проверяют базовые CRUD-операции - (Create, Read, Update, Delete).
/// </summary>
public class InMemoryRepositoryTests
{
    /// <summary>
    /// Проверяет, что BankAccountRepository корректно добавляет и находит счёт.
    /// </summary>
    [Fact]
    public void BankAccountRepository_AddAndGet_ShouldWorkCorrectly()
    {
        // Arrange
        var repository = new InMemoryBankAccountRepository();
        var account = new BankAccount("Тестовый счёт");

        // Act
        repository.Add(account);
        var foundAccount = repository.GetById(account.Id);
        var allAccounts = repository.GetAll();

        // Assert
        Assert.NotNull(foundAccount);
        Assert.Equal(account.Name, foundAccount.Name);
        Assert.Single(allAccounts); // Проверяем, что в списке всего один счёт
    }
    
    /// <summary>
    /// Проверяет, что BankAccountRepository корректно обновляет данные счёта.
    /// </summary>
    [Fact]
    public void BankAccountRepository_Update_ShouldChangeData()
    {
        // Arrange
        var repository = new InMemoryBankAccountRepository();
        var account = new BankAccount("Старое имя");
        repository.Add(account);
        
        // Act
        account.Name = "Новое имя";
        repository.Update(account);
        var updatedAccount = repository.GetById(account.Id);

        // Assert
        Assert.NotNull(updatedAccount);
        Assert.Equal("Новое имя", updatedAccount.Name);
    }

    /// <summary>
    /// Проверяет, что CategoryRepository корректно добавляет и находит категорию.
    /// </summary>
    [Fact]
    public void CategoryRepository_AddAndGet_ShouldWorkCorrectly()
    {
        // Arrange
        var repository = new InMemoryCategoryRepository();
        var category = new Category(CategoryType.Expense, "Продукты");

        // Act
        repository.Add(category);
        var foundCategory = repository.GetById(category.Id);
        var allCategories = repository.GetAll();

        // Assert
        Assert.NotNull(foundCategory);
        Assert.Equal(category.Name, foundCategory.Name);
        Assert.Single(allCategories);
    }

    /// <summary>
    /// Проверяет, что OperationRepository корректно добавляет и находит операцию.
    /// </summary>
    [Fact]
    public void OperationRepository_AddAndGet_ShouldWorkCorrectly()
    {
        // Arrange
        var repository = new InMemoryOperationRepository();
        var operation = new Operation(OperationType.Income, Guid.NewGuid(), 1000m, DateTime.Now, "Аванс", Guid.NewGuid());

        // Act
        repository.Add(operation);
        var foundOperation = repository.GetById(operation.Id);
        var allOperations = repository.GetAll();

        // Assert
        Assert.NotNull(foundOperation);
        Assert.Equal(operation.Description, foundOperation.Description);
        Assert.Single(allOperations);
    }
}