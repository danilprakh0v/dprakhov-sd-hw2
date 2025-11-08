/*
 * Проект: FinanceTracker
 * Файл: IAnalyticsService.cs
 * Расположение: FinanceTracker.Application/Interfaces/
 * Назначение: Интерфейс сервиса аналитики с расширенной функциональностью.
 * ========================================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
namespace FinanceTracker.Application.Interfaces;

using System;
using System.Collections.Generic;
using FinanceTracker.Core.Entities;

/// <summary>
/// Интерфейс сервиса для выполнения аналитических операций над финансовыми данными.
/// Предоставляет методы для получения различных отчетов и статистики.
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Возвращает разницу между доходами и расходами за указанный период.
    /// </summary>
    /// <param name="startDate"> Начало периода (включительно).</param>
    /// <param name="endDate"> Конец периода (включительно) </param>
    /// <returns> Чистая прибыль (доходы минус расходы).
    /// Может быть отрицательной. </returns>
    decimal GetIncomeVsExpenses(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Группирует операции по категориям за указанный период.
    /// </summary>
    /// <param name="startDate"> Начало периода. </param>
    /// <param name="endDate"> Конец периода. </param>
    /// <returns> Словарь: название категории → сумма операций (доходы положительны, расходы отрицательны). </returns>
    Dictionary<string, decimal> GroupByCategory(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Получает детальную аналитику по всем категориям.
    /// </summary>
    /// <returns> Словарь, где ключ - название категории, значение - сводная информация по категории. </returns>
    Dictionary<string, CategorySummary> GetDetailedCategoryAnalysis();

    /// <summary>
    /// Получает общую статистику по всем счетам и операциям.
    /// </summary>
    /// <returns> Объект со сводной статистикой по системе. </returns>
    OverallStatistics GetOverallStatistics();

    /// <summary>
    /// Выполняет автоматический пересчёт балансов всех счетов на основе истории операций.
    /// </summary>
    void RecalculateBalances();
}

/// <summary>
/// Сводная информация по категории для детальной аналитики.
/// </summary>
public class CategorySummary
{
    /// <summary>
    /// Тип категории (доход или расход).
    /// </summary>
    public CategoryType CategoryType { get; set; }

    /// <summary>
    /// Общий доход по категории.
    /// </summary>
    public decimal TotalIncome { get; set; }

    /// <summary>
    /// Общий расход по категории.
    /// </summary>
    public decimal TotalExpense { get; set; }

    /// <summary>
    /// Количество операций в категории.
    /// </summary>
    public int OperationCount { get; set; }

    /// <summary>
    /// Средняя сумма операций в категории.
    /// </summary>
    public decimal AverageAmount { get; set; }
}

/// <summary>
/// Общая статистика по всем счетам и операциям.
/// </summary>
public class OverallStatistics
{
    /// <summary>
    /// Общее количество счетов в системе.
    /// </summary>
    public int TotalAccounts { get; set; }

    /// <summary>
    /// Общий баланс по всем счетам.
    /// </summary>
    public decimal TotalBalance { get; set; }

    /// <summary>
    /// Общее количество операций в системе.
    /// </summary>
    public int TotalOperations { get; set; }

    /// <summary>
    /// Общий доход по всем операциям.
    /// </summary>
    public decimal TotalIncome { get; set; }

    /// <summary>
    /// Общий расход по всем операциям.
    /// </summary>
    public decimal TotalExpense { get; set; }

    /// <summary>
    /// Чистая прибыль (TotalIncome + TotalExpense, где TotalExpense отрицательный).
    /// </summary>
    public decimal NetProfit { get; set; }

    /// <summary>
    /// Топ категорий по объему операций.
    /// Каждый элемент содержит: название категории, общую сумму, количество операций.
    /// </summary>
    public List<(string Name, decimal Total, int Count)> TopCategories { get; set; } = new();
}