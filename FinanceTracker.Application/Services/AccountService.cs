/*
 * Проект: FinanceTracker
 * Файл: AccountService.cs
 * Расположение: FinanceTracker.Application/Services/
 * Назначение: Реализация фасада для работы со счетами.
 * - Инкапсулирует бизнес-логику создания, получения и удаления счетов.
 * ====================================================================
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
/// Сервис для управления банковскими счетами.
/// Реализует паттерн Фасад, предоставляя высокоуровневый API для UI.
/// Следует принципам:
/// - SRP: только операции со счетами,
/// - DIP: зависит от IBankAccountRepository,
/// - GRASP: Information Expert — знает, как работать со счетами.
/// </summary>
public class AccountService : IAccountService
{
    private readonly IBankAccountRepository _repository;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="AccountService"/>.
    /// </summary>
    /// <param name="repository">Репозиторий для доступа к данным счетов. Инжектируется через DI.</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если repository равен null.</exception>
    public AccountService(IBankAccountRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public BankAccount CreateAccount(string name)
    {
        var account = new BankAccount(name);
        _repository.Add(account);
        return account;
    }

    public void DeleteAccount(Guid id)
    {
        _repository.Delete(id);
    }

    public BankAccount? GetById(Guid id)
    {
        return _repository.GetById(id);
    }

    public IEnumerable<BankAccount> GetAll()
    {
        return _repository.GetAll();
    }

    public void UpdateAccount(BankAccount account)
    {
        _repository.Update(account ?? throw new ArgumentNullException(nameof(account)));
    }

    public void SetBalanceManually(Guid accountId, decimal newBalance)
    {
        var account = GetById(accountId) ?? throw new InvalidOperationException("Счёт не найден.");
        account.SetBalance(newBalance);
        _repository.Update(account);
    }
}