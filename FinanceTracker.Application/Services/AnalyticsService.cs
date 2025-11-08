/*
 * Проект: FinanceTracker
 * Файл: AnalyticsService.cs
 * Расположение: FinanceTracker.Application/Services/
 * Назначение: Сервис аналитики с исправленной логикой пересчёта балансов.
 * =======================================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
namespace FinanceTracker.Application.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using FinanceTracker.Application.Interfaces; // ← Обязательно!
using FinanceTracker.Core.Entities;
using FinanceTracker.Core.Interfaces;

/// <summary>
/// Сервис для выполнения аналитических операций над финансовыми данными.
/// С исправленной логикой пересчёта балансов.
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly IOperationRepository _operationRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IBankAccountRepository _accountRepo;

    public AnalyticsService(
        IOperationRepository operationRepo,
        ICategoryRepository categoryRepo,
        IBankAccountRepository accountRepo)
    {
        _operationRepo = operationRepo ?? throw new ArgumentNullException(nameof(operationRepo));
        _categoryRepo = categoryRepo ?? throw new ArgumentNullException(nameof(categoryRepo));
        _accountRepo = accountRepo ?? throw new ArgumentNullException(nameof(accountRepo));
    }

    public decimal GetIncomeVsExpenses(DateTime startDate, DateTime endDate)
    {
        var operations = _operationRepo.GetAll()
            .Where(o => o.Date >= startDate && o.Date <= endDate);

        var enumerable = operations as Operation[] ?? operations.ToArray();
        var income = enumerable.Where(o => o.Type == OperationType.Income).Sum(o => o.Amount);
        var expenses = enumerable.Where(o => o.Type == OperationType.Expense).Sum(o => o.Amount);
        return income + expenses;
    }

    public Dictionary<string, decimal> GroupByCategory(DateTime startDate, DateTime endDate)
    {
        var operations = _operationRepo.GetAll()
            .Where(o => o.Date >= startDate && o.Date <= endDate);

        var categories = _categoryRepo.GetAll().ToDictionary(c => c.Id, c => c.Name);

        return operations
            .GroupBy(o => o.CategoryId)
            .ToDictionary(
                g => categories.TryGetValue(g.Key, out var name) ? name : $"Неизвестная ({g.Key})",
                g => g.Sum(o => o.Amount)
            );
    }

    public Dictionary<string, CategorySummary> GetDetailedCategoryAnalysis()
    {
        var categories = _categoryRepo.GetAll().ToDictionary(c => c.Id);
        var operations = _operationRepo.GetAll().ToList();
        
        var result = new Dictionary<string, CategorySummary>();
        
        foreach (var category in categories.Values)
        {
            var categoryOps = operations.Where(o => o.CategoryId == category.Id).ToList();
            var income = categoryOps.Where(o => o.Type == OperationType.Income).Sum(o => o.Amount);
            var expense = categoryOps.Where(o => o.Type == OperationType.Expense).Sum(o => o.Amount);
            
            result[category.Name] = new CategorySummary
            {
                CategoryType = category.Type,
                TotalIncome = income,
                TotalExpense = expense,
                OperationCount = categoryOps.Count,
                AverageAmount = categoryOps.Count > 0 ? categoryOps.Average(o => Math.Abs(o.Amount)) : 0
            };
        }
        
        return result;
    }

    public OverallStatistics GetOverallStatistics()
    {
        var accounts = _accountRepo.GetAll().ToList();
        var operations = _operationRepo.GetAll().ToList();
        var categories = _categoryRepo.GetAll().ToDictionary(c => c.Id, c => c.Name);
        
        var totalBalance = accounts.Sum(a => a.Balance);
        var totalIncome = operations.Where(o => o.Type == OperationType.Income).Sum(o => o.Amount);
        var totalExpense = operations.Where(o => o.Type == OperationType.Expense).Sum(o => o.Amount);
        
        var categoryStats = operations
            .GroupBy(o => o.CategoryId)
            .Select(g => new 
            {
                CategoryId = g.Key,
                Total = g.Sum(o => o.Amount),
                Count = g.Count()
            })
            .ToDictionary(x => x.CategoryId, x => (x.Total, x.Count));
        
        var readableStats = categoryStats
            .Select(x => (
                Name: categories.TryGetValue(x.Key, out var name) ? name : $"Неизвестная ({x.Key})",
                x.Value.Total,
                x.Value.Count
            ))
            .OrderByDescending(x => Math.Abs(x.Total))
            .ToList();
        
        return new OverallStatistics
        {
            TotalAccounts = accounts.Count,
            TotalBalance = totalBalance,
            TotalOperations = operations.Count,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            NetProfit = totalIncome + totalExpense,
            TopCategories = readableStats
        };
    }

    public void RecalculateBalances()
    {
        var accounts = _accountRepo.GetAll().ToDictionary(a => a.Id);
        var operations = _operationRepo.GetAll().ToList();

        foreach (var account in accounts.Values)
        {
            var accountOperations = operations
                .Where(o => o.BankAccountId == account.Id)
                .ToList();
            
            decimal recalculatedBalance = accountOperations.Sum(o => o.Amount);
            account.SetBalance(recalculatedBalance);
            _accountRepo.Update(account);
        }
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[СТАТУС] Балансы всех счетов корректно пересчитаны.");
        Console.ResetColor();
    }
}