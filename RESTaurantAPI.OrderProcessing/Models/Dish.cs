#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations.Schema;

namespace RESTaurantAPI.OrderProcessing.Models;

/// <summary>
/// Класс, содержащий информацию о блюде.
/// </summary>
public record Dish {
    /// <summary>
    /// Идентификатор блюда.
    /// </summary>
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Название блюда.
    /// </summary>
    [Column("name")]
    public string Name { get; set; }

    /// <summary>
    /// Описание блюда.
    /// </summary>
    [Column("description")]
    public string Description { get; set; }

    /// <summary>
    /// Стоимость блюда.
    /// </summary>
    [Column("price")]
    public decimal Price { get; set; }

    /// <summary>
    /// Количество блюд, доступных к заказу.
    /// </summary>
    [Column("quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// Дата и время создания блюда.
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата и время последнего обновления блюда.
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
