#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;

namespace RESTaurantAPI.OrderProcessing.Requests;

/// <summary>
/// Модель запроса на создание заказа.
/// </summary>
public record CreateOrderRequest {
    /// <summary>
    /// Идентификатор пользователя, создающего заказ.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int UserId { get; init; }

    /// <summary>
    /// Статус заказа.
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 1)]
    [RegularExpression(@"^(Pending|Processing|Completed|Cancelled)$")]
    public string Status { get; } = "Pending";
    
    /// <summary>
    /// Особые пожелания к заказу.
    /// </summary>
    public string SpecialRequests { get; init; } = string.Empty;
    
    /// <summary>
    /// Список блюд в заказе.
    /// </summary>
    [Required]
    public List<OrderDishRequest> OrderDishes { get; init; }
}
