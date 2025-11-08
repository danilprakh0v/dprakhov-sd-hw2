/*
 * Проект: FinanceTracker
 * Файл: CategoryServiceTests.cs
 * Расположение: FinanceTracker.Tests/Services/
 * Назначение: Юнит-тесты для CategoryService — проверка создания и получения категорий.
  * ====================================================================================
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
using System.Collections.Generic;
using System.Linq;

namespace FinanceTracker.Tests.Services;

/// <summary>
/// Тесты для сервиса управления категориями.
/// </summary>
public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockRepo;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockRepo = new Mock<ICategoryRepository>();
        _service = new CategoryService(_mockRepo.Object);
    }

    /// <summary>
    /// Проверяет успешное создание категории с валидными параметрами.
    /// </summary>
    [Fact]
    public void CreateCategory_WithValidNameAndType_ShouldCreateCategory()
    {
        // Arrange
        const string categoryName = "Зарплата";
        const CategoryType type = CategoryType.Income;

        // Act
        var result = _service.CreateCategory(type, categoryName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryName, result.Name);
        Assert.Equal(type, result.Type);
        _mockRepo.Verify(r => r.Add(It.Is<Category>(c => c.Name == categoryName)), Times.Once);
    }
    
    /// <summary>
    /// Проверяет, что создание категории с невалидным именем вызывает исключение.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateCategory_WithInvalidName_ShouldThrowArgumentNullException(string invalidName)
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => _service.CreateCategory(CategoryType.Expense, invalidName));
    }

    /// <summary>
    /// Проверяет, что сервис возвращает все категории из репозитория.
    /// </summary>
    [Fact]
    public void GetAll_WhenCategoriesExist_ShouldReturnAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new(CategoryType.Income, "Зарплата"),
            new(CategoryType.Expense, "Кафе")
        };
        _mockRepo.Setup(r => r.GetAll()).Returns(categories);

        // Act
        var result = _service.GetAll();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Name == "Зарплата");
    }
}