/*
 * Проект: FinanceTracker
 * Файл: Category.cs
 * Расположение: FinanceTracker.Core/Entities/
 * Назначение: Доменная сущность - "Категория".
 * Определяет тип операции (доход/расход) и название.
 * ======================================================
 * Дисциплина: "Конструирование программного обеспечения"
 * Группа: БПИ246-1
 * Студент: Прахов Данил
 * Дата: 09.11.2025
 */
namespace FinanceTracker.Core.Entities;

/// <summary>
/// Представляет категорию для операций - (доходов или расходов).
/// </summary>
public class Category
{
    /// <summary>
    /// Уникальный идентификатор категории.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Тип категории: доход или расход.
    /// </summary>
    public CategoryType Type { get; private set; }

    /// <summary>
    /// Название категории - (Например: "Зарплата", "Ресторан" и т.д.).
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Приватный конструктор для сериализации.
    /// </summary>
    private Category() { }

    /// <summary>
    /// Создает новую категорию операции.
    /// </summary>
    /// <param name="type"> Тип категории - (доход или расход). </param>
    /// <param name="name"> Название категории. Не может быть null или пустой строкой.</param>
    /// <exception cref="ArgumentNullException"> Выбрасывается, если name равно null. </exception>
    /// <exception cref="ArgumentException"> Выбрасывается, если name пустое. </exception>
    public Category(CategoryType type, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Название категории не может быть пустым.");

        Id = Guid.NewGuid();
        Type = type;
        Name = name;
    }

    /// <summary>
    /// Обновляет название категории.
    /// </summary>
    /// <param name="newName"> Новое название.
    /// Не может быть null или пустой строкой. </param>
    /// <exception cref="ArgumentNullException"> Выбрасывается, если newName равно null. </exception>
    /// <exception cref="ArgumentException"> Выбрасывается, если newName пустое. </exception>
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentNullException(nameof(newName), "Новое название категории не может быть пустым.");

        Name = newName;
    }
}

/// <summary>
/// Перечисление типов категорий - (доход либо расход).
/// </summary>
public enum CategoryType
{
    Income,
    Expense
}