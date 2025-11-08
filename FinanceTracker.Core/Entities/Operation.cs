/*
 * Проект: FinanceTracker
 * Файл: Operation.cs
 * Расположение: FinanceTracker.Core/Entities/
 * Назначение: Доменная сущность "Операция".
 * Представляет собой объект, транзакцию - (доход или расход).
 * ===========================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
namespace FinanceTracker.Core.Entities;

using System;

/// <summary>
/// Представляет финансовую операцию - (доход или расход).
/// </summary>
public class Operation
{
    /// <summary>
    /// Уникальный идентификатор операции.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary> 
    /// Тип операции: (доход или расход).
    /// </summary>
    public OperationType Type { get; private set; }

    /// <summary>
    /// Ссылка на счет, к которому относится операция.
    /// </summary>
    public Guid BankAccountId { get; private set; }

    /// <summary>
    /// Сумма операции.
    /// Положительная для дохода, отрицательная для расхода.
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// Дата операции.
    /// </summary>
    public DateTime Date { get; private set; }

    /// <summary>
    /// Описание операции (необязательное поле).
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Ссылка на категорию, к которой относится операция.
    /// </summary>
    public Guid CategoryId { get; private set; }

    /// <summary>
    /// Приватный конструктор для сериализации.
    /// </summary>
    private Operation() { }

    /// <summary>
    /// Создает новую операцию.
    /// </summary>
    /// <param name="type"> Тип операции - (доход или расход). </param>
    /// <param name="bankAccountId"> Идентификатор соответсвующего счёта. </param>
    /// <param name="amount"> Сумма операции.
    /// Для расхода должна быть отрицательной, для дохода — положительной. </param>
    /// <param name="date"> Дата операции. </param>
    /// <param name="description"> Описание операции - (необязательный элемент). </param>
    /// <param name="categoryId"> Идентификатор категории. </param>
    /// <exception cref="ArgumentException"> Выбрасывается, если amount имеет неправильный знак для типа операции. </exception>
    public Operation(OperationType type, Guid bankAccountId, decimal amount, DateTime date, string? description, Guid categoryId)
    {
        if (type == OperationType.Income && amount <= 0)
            throw new ArgumentException("Сумма дохода должна быть положительной.", nameof(amount));
        if (type == OperationType.Expense && amount >= 0)
            throw new ArgumentException("Сумма расхода должна быть отрицательной.", nameof(amount));

        Id = Guid.NewGuid();
        Type = type;
        BankAccountId = bankAccountId;
        Amount = amount;
        Date = date;
        Description = description;
        CategoryId = categoryId;
    }

    /// <summary>
    /// Обновляет описание операции.
    /// </summary>
    /// <param name="newDescription"> Новое описание. </param>
    public void UpdateDescription(string? newDescription)
    {
        Description = newDescription;
    }

    /// <summary>
    /// Обновляет сумму операции.
    /// Проверяет, что знак соответствует типу операции.
    /// </summary>
    /// <param name="newAmount"> Новая сумма. </param>
    /// <exception cref="ArgumentException"> Выбрасывается, если newAmount имеет неправильный знак для типа операции. </exception>
    public void UpdateAmount(decimal newAmount)
    {
        if (Type == OperationType.Income && newAmount <= 0)
            throw new ArgumentException("Сумма дохода должна быть положительной.", nameof(newAmount));
        if (Type == OperationType.Expense && newAmount >= 0)
            throw new ArgumentException("Сумма расхода должна быть отрицательной.", nameof(newAmount));

        Amount = newAmount;
    }
}

/// <summary>
/// Перечисление типов операций.
/// </summary>
public enum OperationType
{
    Income,
    Expense
}