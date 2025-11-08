/*
 * Проект: FinanceTracker
 * Файл: IOperationService.cs
 * Расположение: FinanceTracker.Application/Interfaces/
 * Назначение: Интерфейс сервиса для работы с операциями.
 * ======================================================
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
/// Интерфейс сервиса для управления финансовыми операциями.
/// Обеспечивает создание, редактирование и удаление операций с валидацией.
/// </summary>
public interface IOperationService
{
    Operation AddOperation(OperationType type, Guid bankAccountId, decimal amount, DateTime date, string? description, Guid categoryId);
    void DeleteOperation(Guid id);
    Operation? GetById(Guid id);
    IEnumerable<Operation> GetAll();
    void UpdateOperation(Operation operation);
}