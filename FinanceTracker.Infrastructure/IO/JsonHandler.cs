/*
 * Проект: FinanceTracker
 * Файл: JsonHandler.cs
 * Расположение: FinanceTracker.Infrastructure/IO/
 * Назначение: Обработчик JSON-файлов.
 * ======================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
namespace FinanceTracker.Infrastructure.IO;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using FinanceTracker.Core.Entities;
using FinanceTracker.Core.Interfaces;

/// <summary>
/// Обычный обработчик JSON-файлов с поддержкой Unicode.
/// </summary>
public class JsonHandler
{
    private readonly IBankAccountRepository _accountRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IOperationRepository _operationRepo;

    public JsonHandler(
        IBankAccountRepository accountRepo,
        ICategoryRepository categoryRepo,
        IOperationRepository operationRepo)
    {
        _accountRepo = accountRepo ?? throw new ArgumentNullException(nameof(accountRepo));
        _categoryRepo = categoryRepo ?? throw new ArgumentNullException(nameof(categoryRepo));
        _operationRepo = operationRepo ?? throw new ArgumentNullException(nameof(operationRepo));
    }

    public void Import(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл не найден: {filePath}");

        var json = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) // Поддержка кириллицы
        };

        var dto = JsonSerializer.Deserialize<FinanceDataDto>(json, options)
                  ?? throw new InvalidOperationException("Не удалось десериализовать JSON.");

        // Очистка текущих данных
        foreach (var acc in _accountRepo.GetAll().ToList()) _accountRepo.Delete(acc.Id);
        foreach (var cat in _categoryRepo.GetAll().ToList()) _categoryRepo.Delete(cat.Id);
        foreach (var op in _operationRepo.GetAll().ToList()) _operationRepo.Delete(op.Id);

        // Восстановление счетов
        foreach (var accDto in dto.Accounts)
        {
            var account = (BankAccount)Activator.CreateInstance(typeof(BankAccount), true)!;
            typeof(BankAccount).GetProperty("Id")!.SetValue(account, accDto.Id);
            typeof(BankAccount).GetProperty("Name")!.SetValue(account, accDto.Name);
            typeof(BankAccount).GetProperty("Balance")!.SetValue(account, accDto.Balance);
            _accountRepo.Add(account);
        }

        // Восстановление категорий (с проверкой на уникальность)
        var uniqueCategories = new Dictionary<string, CategoryDto>();
        foreach (var catDto in dto.Categories)
        {
            var key = $"{catDto.Type}_{catDto.Name}";
            if (!uniqueCategories.ContainsKey(key))
            {
                uniqueCategories[key] = catDto;
                var category = (Category)Activator.CreateInstance(typeof(Category), true)!;
                typeof(Category).GetProperty("Id")!.SetValue(category, catDto.Id);
                typeof(Category).GetProperty("Type")!.SetValue(category, catDto.Type);
                typeof(Category).GetProperty("Name")!.SetValue(category, catDto.Name);
                _categoryRepo.Add(category);
            }
        }

        // Восстановление операций
        foreach (var opDto in dto.Operations)
        {
            var operation = (Operation)Activator.CreateInstance(typeof(Operation), true)!;
            typeof(Operation).GetProperty("Id")!.SetValue(operation, opDto.Id);
            typeof(Operation).GetProperty("Type")!.SetValue(operation, opDto.Type);
            typeof(Operation).GetProperty("BankAccountId")!.SetValue(operation, opDto.BankAccountId);
            typeof(Operation).GetProperty("Amount")!.SetValue(operation, opDto.Amount);
            typeof(Operation).GetProperty("Date")!.SetValue(operation, opDto.Date);
            typeof(Operation).GetProperty("Description")!.SetValue(operation, opDto.Description ?? string.Empty);
            typeof(Operation).GetProperty("CategoryId")!.SetValue(operation, opDto.CategoryId);
            _operationRepo.Add(operation);
        }
    }

    public void Export(string filePath)
    {
        var dto = new FinanceDataDto
        {
            Accounts = _accountRepo.GetAll()
                .Select(a => new BankAccountDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Balance = a.Balance
                })
                .ToList(),

            Categories = _categoryRepo.GetAll()
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Type = c.Type,
                    Name = c.Name
                })
                .ToList(),

            Operations = _operationRepo.GetAll()
                .Select(o => new OperationDto
                {
                    Id = o.Id,
                    Type = o.Type,
                    BankAccountId = o.BankAccountId,
                    Amount = o.Amount,
                    Date = o.Date,
                    Description = o.Description,
                    CategoryId = o.CategoryId
                })
                .ToList()
        };

        var options = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) 
        };
        
        var json = JsonSerializer.Serialize(dto, options);
        File.WriteAllText(filePath, json);
    }
}