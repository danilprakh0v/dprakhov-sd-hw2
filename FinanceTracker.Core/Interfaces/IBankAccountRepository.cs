/*
 * Проект: FinanceTracker
 * Файл: IBankAccountRepository.cs
 * Расположение: FinanceTracker.Core/Interfaces/
 * Назначение: Интерфейс репозитория для работы со счетами.
 * ========================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
namespace FinanceTracker.Core.Interfaces;

using System.Collections.Generic;
using FinanceTracker.Core.Entities;

/// <summary>
/// Интерфейс для хранилища банковских счетов.
/// </summary>
public interface IBankAccountRepository
{
    /// <summary>
    /// Добавляет новый счет в хранилище.
    /// </summary>
    /// <param name="account"> Счет для добавления.</param>
    void Add(BankAccount account);

    /// <summary>
    /// Удаляет счет по идентификатору.
    /// </summary>
    /// <param name="id"> Идентификатор счета. </param>
    void Delete(Guid id);

    /// <summary>
    /// Получает счет по идентификатору.
    /// </summary>
    /// <param name="id"> Идентификатор счета. </param>
    /// <returns> Счет, если найден, иначе null. </returns>
    BankAccount? GetById(Guid id);

    /// <summary>
    /// Получает все счета.
    /// </summary>
    /// <returns> Коллекция всех счетов. </returns>
    IEnumerable<BankAccount> GetAll();

    /// <summary>
    /// Обновляет существующий счет.
    /// </summary>
    /// <param name="account"> Счет с обновленными данными.</param>
    void Update(BankAccount account);
}