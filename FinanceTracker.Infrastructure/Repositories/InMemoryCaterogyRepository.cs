/*
 * Проект: FinanceTracker
 * Файл: InMemoryCategoryRepository.cs
 * Расположение: FinanceTracker.Infrastructure/Repositories/
 * Назначение: Реализация репозитория категорий в памяти. Обеспечивает управление категориями доходов и расходов.
 * ============================================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: [Ваше имя]
 * Дата: 06.11.2025
 */
namespace FinanceTracker.Infrastructure.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using FinanceTracker.Core.Entities;
using FinanceTracker.Core.Interfaces;

/// <summary>
/// In-Memory реализация репозитория для категорий.
/// </summary>
public class InMemoryCategoryRepository : ICategoryRepository
{
    private readonly List<Category> _categories = new();

    public void Add(Category category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));
        _categories.Add(category);
    }

    public void Delete(Guid id)
    {
        _categories.RemoveAll(c => c.Id == id);
    }

    public Category? GetById(Guid id)
    {
        return _categories.FirstOrDefault(c => c.Id == id);
    }

    public IEnumerable<Category> GetAll()
    {
        return _categories.AsReadOnly();
    }

    public void Update(Category category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        var index = _categories.FindIndex(c => c.Id == category.Id);
        if (index >= 0)
            _categories[index] = category;
    }
}