/*
 * Проект: FinanceTracker
 * Файл: ICategoryRepository.cs
 * Расположение: FinanceTracker.Core/Interfaces/
 * Назначение: Интерфейс репозитория для работы с категориями. 
 * ===========================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
namespace FinanceTracker.Core.Interfaces;

using System.Collections.Generic;
using FinanceTracker.Core.Entities;

/// <summary>
/// Интерфейс хранилища для управления категориями.
/// Обеспечивает абстракцию над хранилищем категорий - (доходов и расходов).
/// </summary>
public interface ICategoryRepository
{
    /// <summary>
    /// Добавляет новую категорию в хранилище.
    /// </summary>
    /// <param name="category"> Категория для добавления.
    /// Не может быть null. </param>
    /// <exception cref="System.ArgumentNullException"> Выбрасывается, если category равно null. </exception>
    void Add(Category category);

    /// <summary>
    /// Удаляет категорию по её уникальному идентификатору.
    /// </summary>
    /// <param name="id"> Уникальный идентификатор категории. </param>
    void Delete(Guid id);

    /// <summary>
    /// Получает категорию по уникальному идентификатору.
    /// </summary>
    /// <param name="id"> Уникальный идентификатор категории. </param>
    /// <returns> Категория, если найдена; иначе null. </returns>
    Category? GetById(Guid id);

    /// <summary>
    /// Получает все категории из хранилища.
    /// </summary>
    /// <returns> Коллекция всех категорий. </returns>
    IEnumerable<Category> GetAll();

    /// <summary>
    /// Обновляет существующую категорию в хранилище.
    /// </summary>
    /// <param name="category"> Категория с обновлёнными данными.
    /// Не может быть null. </param>
    /// <exception cref="System.ArgumentNullException"> Выбрасывается, если category равно null. </exception>
    void Update(Category category);
}