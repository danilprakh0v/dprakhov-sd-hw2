/*
 * Проект: FinanceTracker
 * Файл: CategoryService.cs
 * Расположение: FinanceTracker.Application/Services/
 * Назначение: Реализация фасадного интерфейса для работы с категориями доходов и расходов.
 * ========================================================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
namespace FinanceTracker.Application.Services;

using System;
using System.Collections.Generic;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Core.Entities;
using FinanceTracker.Core.Interfaces;

/// <summary>
/// Сервис для управления категориями (доходов и расходов).
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public Category CreateCategory(CategoryType type, string name)
    {
        var category = new Category(type, name);
        _repository.Add(category);
        return category;
    }

    public void DeleteCategory(Guid id)
    {
        _repository.Delete(id);
    }

    public Category? GetById(Guid id)
    {
        return _repository.GetById(id);
    }

    public IEnumerable<Category> GetAll()
    {
        return _repository.GetAll();
    }

    public void UpdateCategory(Category category)
    {
        _repository.Update(category ?? throw new ArgumentNullException(nameof(category)));
    }
}