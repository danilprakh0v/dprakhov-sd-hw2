/*
 * Проект: FinanceTracker
 * Файл: JsonHandlerTests.cs
 * Расположение: FinanceTracker.Tests/Infrastructure/
 * Назначение: Юнит-тесты для импорта и экспорта в JSON, включая обработку ошибок.
 * ===============================================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
using Moq;
using Xunit;
using FinanceTracker.Infrastructure.IO;
using FinanceTracker.Core.Interfaces;
using FinanceTracker.Core.Entities;
using System.Text.RegularExpressions;
using System.Text.Json; 

namespace FinanceTracker.Tests.Infrastructure;

/// <summary>
/// Тесты для обработчика JSON-файлов.
/// </summary>
public class JsonHandlerTests
{
    private readonly Mock<IBankAccountRepository> _mockAccRepo;
    private readonly Mock<ICategoryRepository> _mockCatRepo;
    private readonly Mock<IOperationRepository> _mockOpRepo;
    private readonly JsonHandler _handler;

    public JsonHandlerTests()
    {
        _mockAccRepo = new Mock<IBankAccountRepository>();
        _mockCatRepo = new Mock<ICategoryRepository>();
        _mockOpRepo = new Mock<IOperationRepository>();
        _handler = new JsonHandler(_mockAccRepo.Object, _mockCatRepo.Object, _mockOpRepo.Object);
    }

    /// <summary>
    /// Проверяет, что при экспорте пустых данных создаётся корректный JSON-файл.
    /// </summary>
    [Fact]
    public void Export_WhenDataIsEmpty_ShouldCreateValidJson()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            // Act
            _handler.Export(tempFile);
            var content = File.ReadAllText(tempFile);
            var compactContent = Regex.Replace(content, @"\s+", "");

            // Assert
            Assert.True(File.Exists(tempFile));
            Assert.Contains("\"Accounts\":[]", compactContent);
        }
        finally { if (File.Exists(tempFile)) File.Delete(tempFile); }
    }

    /// <summary>
    /// Проверяет, что при экспорте существующих данных они корректно сохраняются в JSON.
    /// </summary>
    [Fact]
    public void Export_WhenDataExists_ShouldCreateJsonWithData()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        _mockAccRepo.Setup(r => r.GetAll()).Returns(new List<BankAccount> { new("Test Acc") });
        try
        {
            // Act
            _handler.Export(tempFile);
            var content = File.ReadAllText(tempFile);
            var compactContent = Regex.Replace(content, @"\s+", "");

            // Assert
            Assert.Contains("\"Name\":\"TestAcc\"", compactContent);
        }
        finally { if (File.Exists(tempFile)) File.Delete(tempFile); }
    }

    /// <summary>
    /// Проверяет, что при импорте из валидного JSON-файла вызываются методы добавления в репозиториях.
    /// </summary>
    [Fact]
    public void Import_WithValidJson_ShouldCallAddOnRepositories()
    {
        // Arrange
        var accountId = Guid.NewGuid().ToString();
        var categoryId = Guid.NewGuid().ToString();
        var operationId = Guid.NewGuid().ToString();
        var json = $@"{{""Accounts"":[{{""Id"":""{accountId}"",""Name"":""Test Acc"",""Balance"":100}}],""Categories"":[{{""Id"":""{categoryId}"",""Name"":""Test Cat"",""Type"":1}}],""Operations"":[{{""Id"":""{operationId}"",""Type"":0,""BankAccountId"":""{accountId}"",""Amount"":50,""CategoryId"":""{categoryId}""}}]}}";
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, json);
        try
        {
            // Act
            _handler.Import(tempFile);
            
            // Assert
            _mockAccRepo.Verify(r => r.Add(It.IsAny<BankAccount>()), Times.Once);
            _mockCatRepo.Verify(r => r.Add(It.IsAny<Category>()), Times.Once);
            _mockOpRepo.Verify(r => r.Add(It.IsAny<Operation>()), Times.Once);
        }
        finally { if (File.Exists(tempFile)) File.Delete(tempFile); }
    }

    /// <summary>
    /// Проверяет, что при импорте из несуществующего файла выбрасывается исключение.
    /// </summary>
    [Fact]
    public void Import_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => _handler.Import(nonExistentFile));
    }

    /// <summary>
    /// Проверяет, что при импорте из файла с некорректным JSON выбрасывается исключение.
    /// </summary>
    [Fact]
    public void Import_WithInvalidJson_ShouldThrowJsonException()
    {
        // Arrange
        var invalidJson = @"{ ""Accounts"": [ { ""Name"": ""Test"" }"; // "Сломанный" JSON
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, invalidJson);
        try
        {
            // Act & Assert
            Assert.Throws<JsonException>(() => _handler.Import(tempFile));
        }
        finally { if (File.Exists(tempFile)) File.Delete(tempFile); }
    }
}