/*
 * Проект: FinanceTracker
 * Файл: OperationService.cs
 * Расположение: FinanceTracker.Application/Services/
 * Назначение: Реализация фасадного интерфейса для работы с финансовыми операциями.
 * ================================================================================
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
/// Сервис для управления финансовыми операциями (доходами и расходами).
/// </summary>
public class OperationService : IOperationService
{
    private readonly IOperationRepository _repository;

    public OperationService(IOperationRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public Operation AddOperation(OperationType type, Guid bankAccountId, decimal amount, DateTime date, string? description, Guid categoryId)
    {
        var operation = new Operation(type, bankAccountId, amount, date, description, categoryId);
        _repository.Add(operation);
        return operation;
    }

    public void DeleteOperation(Guid id)
    {
        _repository.Delete(id);
    }

    public Operation? GetById(Guid id)
    {
        return _repository.GetById(id);
    }

    public IEnumerable<Operation> GetAll()
    {
        return _repository.GetAll();
    }

    public void UpdateOperation(Operation operation)
    {
        _repository.Update(operation ?? throw new ArgumentNullException(nameof(operation)));
    }
}