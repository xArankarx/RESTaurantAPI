using Microsoft.EntityFrameworkCore;
using RESTaurantAPI.Authorization.Models;

namespace RESTaurantAPI.Authorization.Repositories;

/// <summary>
/// Репозиторий для работы с пользователями.
/// </summary>
public class UserRepository : IUserRepository {
    private readonly DatabaseContext _dbContext;

    /// <summary>
    /// Конструктор репозитория.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных.</param>
    public UserRepository(DatabaseContext dbContext) {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<User?> GetUserById(int id) {
        return await _dbContext.user.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<User?> GetUserByEmail(string email) {
        return await _dbContext.user.FirstOrDefaultAsync(u => u.Email == email);
    }
    
    /// <inheritdoc />
    public async Task<User?> GetUserByUsername(string username) {
        return await _dbContext.user.FirstOrDefaultAsync(u => u.Username == username);
    }

    /// <inheritdoc />
    public async Task CreateUser(User user) {
        await _dbContext.user.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }
}
