/*
 * Проект: FinanceTracker
 * Файл: IOperationRepository.cs
 * Расположение: FinanceTracker.Core/Interfaces/
 * Назначение: Интерфейс репозитория для работы с операциями.
 * ==========================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
namespace FinanceTracker.Core.Interfaces;

using System.Collections.Generic;
using FinanceTracker.Core.Entities;

/// <summary>
/// Интерфейс хранилища для управления финансовыми операциями (доходами и расходами).
/// Обеспечивает абстракцию над хранилищем операций.
/// </summary>
public interface IOperationRepository
{
    /// <summary>
    /// Добавляет новую операцию в хранилище.
    /// </summary>
    /// <param name="operation"> Операция для добавления.
    /// Не может быть null. </param>
    /// <exception cref="System.ArgumentNullException"> Выбрасывается, если operation равно null. </exception>
    void Add(Operation operation);

    /// <summary>
    /// Удаляет операцию по её уникальному идентификатору.
    /// </summary>
    /// <param name="id"> Уникальный идентификатор операции. </param>
    void Delete(Guid id);

    /// <summary>
    /// Получает операцию по уникальному идентификатору.
    /// </summary>
    /// <param name="id"> Уникальный идентификатор операции. </param>
    /// <returns> Операция, если найдена, иначе null. </returns>
    Operation? GetById(Guid id);

    /// <summary>
    /// Получает все операции из хранилища.
    /// </summary>
    /// <returns> Коллекция всех операций. </returns>
    IEnumerable<Operation> GetAll();

    /// <summary>
    /// Обновляет существующую операцию в хранилище.
    /// </summary>
    /// <param name="operation"> Операция с обновлёнными данными.
    /// Не может быть null. </param>
    /// <exception cref="System.ArgumentNullException"> Выбрасывается, если operation равно null. </exception>
    void Update(Operation operation);
}