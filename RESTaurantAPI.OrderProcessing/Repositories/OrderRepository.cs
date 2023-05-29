using Microsoft.EntityFrameworkCore;
using RESTaurantAPI.OrderProcessing.Models;

namespace RESTaurantAPI.OrderProcessing.Repositories; 

/// <summary>
/// Репозиторий для работы с заказами.
/// </summary>
public class OrderRepository : IOrderRepository {
    private readonly DatabaseContext _dbContext;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Конструктор репозитория.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных.</param>
    public OrderRepository(DatabaseContext dbContext) {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Order?> GetOrderById(int id) {
        return await _dbContext.order.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task CreateOrder(Order order) {
        await _dbContext.order.AddAsync(order);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateOrder(Order order) {
        _dbContext.order.Update(order);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteOrder(Order order) {
        _dbContext.order.Remove(order);
        await _dbContext.SaveChangesAsync();
    }
    
    /// <inheritdoc />
    public async Task ProcessOrder(Order order) {
        try {
            if (order.OrderDishes.Count == 0) {
                order.Status = "Cancelled";
                order.UpdatedAt = DateTime.UtcNow;
                _dbContext.order.Update(order);
                await _semaphore.WaitAsync();
                try {
                    await _dbContext.SaveChangesAsync();
                }
                finally {
                    _semaphore.Release();
                }
                return;
            }
            order.Status = "Processing";
            order.UpdatedAt = DateTime.UtcNow;
            _dbContext.order.Update(order);
            await _semaphore.WaitAsync();
            try {
                await _dbContext.SaveChangesAsync();
            }
            finally {
                _semaphore.Release();
            }

            await Task.Delay(TimeSpan.FromMilliseconds(order.OrderDishes.Count * 1000 + 1));

            order.Status = "Completed";
            order.UpdatedAt = DateTime.UtcNow;
            _dbContext.order.Update(order);
            await _semaphore.WaitAsync();
            try {
                await _dbContext.SaveChangesAsync();
            }
            finally {
                _semaphore.Release();
            }
        }
        catch (Exception) {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Order>> GetAllOrders() {
        return await _dbContext.order.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Order>> GetOrdersByUserId(int userId) {
        return await _dbContext.order.Where(order => order.UserId == userId).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Order>> GetOrdersByStatus(string status) {
        return await _dbContext.order.Where(order => order.Status == status).ToListAsync();
    }
}
