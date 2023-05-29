using Microsoft.EntityFrameworkCore;
using RESTaurantAPI.Authorization.Models;

namespace RESTaurantAPI.Authorization.Repositories;

/// <summary>
/// Репозиторий для работы с сессиями.
/// </summary>
public class SessionRepository : ISessionRepository {
    private readonly DatabaseContext _dbContext;

    /// <summary>
    /// Конструктор репозитория.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных.</param>
    public SessionRepository(DatabaseContext dbContext) {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Session?> GetSessionByToken(string token) {
        return await _dbContext.session.FirstOrDefaultAsync(s => s.SessionToken == token);
    }

    /// <inheritdoc />
    public async Task CreateSession(Session session) {
        await _dbContext.session.AddAsync(session);
        await _dbContext.SaveChangesAsync();
    }
}
