#pragma warning disable CS8618

using Microsoft.EntityFrameworkCore;

namespace RESTaurantAPI.OrderProcessing.Models; 

/// <summary>
/// Контекст базы данных.
/// </summary>
public class DatabaseContext : DbContext {
    private readonly IConfiguration _configuration;
    
    /// <summary>
    /// Конструктор контекста базы данных.
    /// </summary>
    /// <param name="configuration">Конфигурация приложения.</param>
    public DatabaseContext(IConfiguration configuration) {
        _configuration = configuration;
    }
    
    /// <summary>
    /// Метод, осуществляющий конфигурацию контекста для подключения к базе данных.
    /// </summary>
    /// <param name="optionsBuilder">Параметры подключения к базе данных.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Конфигурация контекста для подключения к базе данных PostgreSQL.
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
    }

    /// <summary>
    /// Таблица блюд.
    /// </summary>
    public DbSet<Dish> dish { get; set; }
    
    /// <summary>
    /// Таблица заказов.
    /// </summary>
    public DbSet<Order> order { get; set; }
    
    /// <summary>
    /// Таблица блюд в заказах.
    /// </summary>
    public DbSet<OrderDish> order_dish { get; set; }
}
