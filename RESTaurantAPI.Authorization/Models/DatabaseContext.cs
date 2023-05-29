#pragma warning disable CS8618

using Microsoft.EntityFrameworkCore;

namespace RESTaurantAPI.Authorization.Models; 

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
    /// Таблица пользователей.
    /// </summary>
    public DbSet<User> user { get; set; }
    
    /// <summary>
    /// Таблица сессий.
    /// </summary>
    public DbSet<Session> session { get; set; }
}