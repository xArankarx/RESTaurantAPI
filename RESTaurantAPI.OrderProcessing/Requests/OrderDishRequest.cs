#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;

namespace RESTaurantAPI.OrderProcessing.Requests;

/// <summary>
/// Модель запроса на добавление блюда в заказ.
/// </summary>
public record OrderDishRequest {
    /// <summary>
    /// Идентификатор блюда.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int DishId { get; init; }
    
    /// <summary>
    /// Количество блюд в заказе.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }
}
