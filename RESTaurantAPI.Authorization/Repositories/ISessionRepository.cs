using RESTaurantAPI.Authorization.Models;

namespace RESTaurantAPI.Authorization.Repositories; 

/// <summary>
/// Интерфейс репозитория для работы с сессиями.
/// </summary>
public interface ISessionRepository {
    /// <summary>
    /// Метод, осуществляющий поиск сессии по токену в базе данных.
    /// </summary>
    /// <param name="token">Токен сессии.</param>
    /// <returns>Сессия или null, если сессия не найдена.</returns>
    Task<Session?> GetSessionByToken(string token);
    
    /// <summary>
    /// Метод, осуществляющий добавление сессии в базу данных.
    /// </summary>
    /// <param name="session">Добавляемая сессия.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task CreateSession(Session session);
}
