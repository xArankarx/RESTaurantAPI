using Microsoft.EntityFrameworkCore;
using RESTaurantAPI.OrderProcessing.Models;

namespace RESTaurantAPI.OrderProcessing.Repositories; 

/// <summary>
/// Репозиторий для работы с блюдами заказа.
/// </summary>
public class OrderDishRepository : IOrderDishRepository {
    private readonly DatabaseContext _dbContext;
    
    /// <summary>
    /// Конструктор репозитория.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных.</param>
    public OrderDishRepository(DatabaseContext dbContext) {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task CreateOrderDish(OrderDish orderDish) {
        await _dbContext.order_dish.AddAsync(orderDish);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteOrderDish(OrderDish orderDish) {
        _dbContext.order_dish.Remove(orderDish);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderDish>> GetOrderDishesByOrderId(int orderId) {
        return await _dbContext.order_dish.Where(orderDish => orderDish.OrderId == orderId).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderDish>> GetOrderDishesByDishId(int dishId) {
        return await _dbContext.order_dish.Where(orderDish => orderDish.DishId == dishId).ToListAsync();
    }
}
