#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;

namespace RESTaurantAPI.OrderProcessing.Requests;

/// <summary>
/// Модель запроса на создание (обновление) блюда.
/// </summary>
public record CreateDishRequest {
    /// <summary>
    /// Название блюда.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; init; }

    /// <summary>
    /// Описание блюда.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// Стоимость блюда.
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; init; }

    /// <summary>
    /// Количество доступных блюд.
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity { get; init; }
}
