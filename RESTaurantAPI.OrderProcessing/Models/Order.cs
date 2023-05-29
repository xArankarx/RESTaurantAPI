#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations.Schema;

namespace RESTaurantAPI.OrderProcessing.Models; 

/// <summary>
/// Класс, содержащий информацию о заказе.
/// </summary>
public record Order {
    /// <summary>
    /// Идентификатор заказа.
    /// </summary>
    [Column("id")]
    public int Id { get; set; }
        
    /// <summary>
    /// Идентификатор пользователя, сделавшего заказ.
    /// </summary>
    [Column("user_id")]
    public int UserId { get; set; }
        
    /// <summary>
    /// Статус заказа.
    /// </summary>
    [Column("status")]
    public string Status { get; set; }
        
    /// <summary>
    /// Особые пожелания к заказу.
    /// </summary>
    [Column("special_requests")]
    public string SpecialRequests { get; set; }
        
    /// <summary>
    /// Дата и время создания заказа.
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
        
    /// <summary>
    /// Дата и время последнего обновления заказа.
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Список блюд в заказе.
    /// </summary>
    [NotMapped] public List<OrderDish> OrderDishes { get; set; } = new();
}
