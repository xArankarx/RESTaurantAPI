using System.ComponentModel.DataAnnotations.Schema;

namespace RESTaurantAPI.OrderProcessing.Models; 

/// <summary>
/// Класс, содержащий информацию о блюде в заказе.
/// </summary>
public record OrderDish {
    /// <summary>
    /// Идентификатор блюда в заказе.
    /// </summary>
    [Column("id")]
    public int Id { get; set; }
        
    /// <summary>
    /// Идентификатор заказа.
    /// </summary>
    [Column("order_id")]
    public int OrderId { get; set; }
        
    /// <summary>
    /// Идентификатор блюда.
    /// </summary>
    [Column("dish_id")]
    public int DishId { get; set; }
        
    /// <summary>
    /// Количество блюд в заказе.
    /// </summary>
    [Column("quantity")]
    public int Quantity { get; set; }
        
    /// <summary>
    /// Общая стоимость блюд в заказе.
    /// </summary>
    [Column("price")]
    public decimal Price { get; set; }
}
