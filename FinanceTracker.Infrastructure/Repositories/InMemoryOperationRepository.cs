/*
 * Проект: FinanceTracker
 * Файл: InMemoryOperationRepository.cs
 * Расположение: FinanceTracker.Infrastructure/Repositories/
 * Назначение: Реализация репозитория операций в памяти. Хранит историю доходов и расходов.
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
/// In-Memory реализация репозитория для финансовых операций.
/// </summary>
public class InMemoryOperationRepository : IOperationRepository
{
    private readonly List<Operation> _operations = new();

    public void Add(Operation operation)
    {
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));
        _operations.Add(operation);
    }

    public void Delete(Guid id)
    {
        _operations.RemoveAll(o => o.Id == id);
    }

    public Operation? GetById(Guid id)
    {
        return _operations.FirstOrDefault(o => o.Id == id);
    }

    public IEnumerable<Operation> GetAll()
    {
        return _operations.AsReadOnly();
    }

    public void Update(Operation operation)
    {
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));

        var index = _operations.FindIndex(o => o.Id == operation.Id);
        if (index >= 0)
            _operations[index] = operation;
    }
}