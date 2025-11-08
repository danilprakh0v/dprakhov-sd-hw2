/*
 * Проект: FinanceTracker
 * Файл: ICategoryService.cs
 * Расположение: FinanceTracker.Application/Interfaces/
 * Назначение: Интерфейс сервиса для работы с категориями.
 * =======================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
namespace FinanceTracker.Application.Interfaces;

using System.Collections.Generic;
using FinanceTracker.Core.Entities;

/// <summary>
/// Интерфейс сервиса для управления категориями (доходов и расходов).
/// Предоставляет унифицированный API для работы с категориями.
/// </summary>
public interface ICategoryService
{
    Category CreateCategory(CategoryType type, string name);
    void DeleteCategory(Guid id);
    Category? GetById(Guid id);
    IEnumerable<Category> GetAll();
    void UpdateCategory(Category category);
}