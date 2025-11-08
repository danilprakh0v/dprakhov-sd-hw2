/*
 * Проект: FinanceTracker
 * Файл: InMemoryBankAccountRepository.cs
 * Расположение: FinanceTracker.Infrastructure/Repositories/
 * Назначение: Реализация репозитория банковских счетов в памяти. Обеспечивает CRUD-операции над счетами без внешней персистентности.
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
/// In-Memory реализация репозитория для банковских счетов.
/// Хранит данные в приватном списке на время жизни приложения.
/// Следует принципу DIP: реализует интерфейс IBankAccountRepository из Core.
/// </summary>
public class InMemoryBankAccountRepository : IBankAccountRepository
{
    private readonly List<BankAccount> _accounts = new();

    /// <inheritdoc />
    public void Add(BankAccount account)
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account));
        _accounts.Add(account);
    }

    /// <inheritdoc />
    public void Delete(Guid id)
    {
        _accounts.RemoveAll(a => a.Id == id);
    }

    /// <inheritdoc />
    public BankAccount? GetById(Guid id)
    {
        return _accounts.FirstOrDefault(a => a.Id == id);
    }

    /// <inheritdoc />
    public IEnumerable<BankAccount> GetAll()
    {
        return _accounts.AsReadOnly();
    }

    /// <inheritdoc />
    public void Update(BankAccount account)
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account));

        var index = _accounts.FindIndex(a => a.Id == account.Id);
        if (index >= 0)
            _accounts[index] = account;
        // Если счёт не найден — игнорируем (можно выбросить исключение, но для In-Memory допустимо)
    }
}