using Microsoft.EntityFrameworkCore;
using RESTaurantAPI.OrderProcessing.Models;

namespace RESTaurantAPI.OrderProcessing.Repositories; 

/// <summary>
/// Репозиторий для работы с блюдами.
/// </summary>
public class DishRepository : IDishRepository {
    private readonly DatabaseContext _dbContext;

    /// <summary>
    /// Конструктор репозитория.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных.</param>
    public DishRepository(DatabaseContext dbContext) {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Dish?> GetDishById(int id) {
        return await _dbContext.dish.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task CreateDish(Dish dish) {
        await _dbContext.dish.AddAsync(dish);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateDish(Dish dish) {
        _dbContext.dish.Update(dish);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteDish(Dish dish) {
        _dbContext.dish.Remove(dish);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Dish>> GetAllDishes() {
        return await _dbContext.dish.ToListAsync();
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<Dish>> GetDishesByName(string name) {
        return await _dbContext.dish.Where(dish => dish.Name == name).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Dish>> GetDishesByAvailability(bool isAvailable) {
        return await _dbContext.dish.Where(dish => dish.Quantity > 0).ToListAsync();
    }
}
