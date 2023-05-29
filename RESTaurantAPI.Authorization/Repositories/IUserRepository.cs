using RESTaurantAPI.Authorization.Models;

namespace RESTaurantAPI.Authorization.Repositories; 

/// <summary>
/// Интерфейс репозитория для работы с пользователями.
/// </summary>
public interface IUserRepository {
    /// <summary>
    /// Метод, осуществляющий поиск пользователя по его идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <returns>Найденный пользователь или null, если пользователь не найден.</returns>
    Task<User?> GetUserById(int id);
    
    /// <summary>
    /// Метод, осуществляющий поиск пользователя по его электронной почте.
    /// </summary>
    /// <param name="email">Электронная почта пользователя.</param>
    /// <returns>Найденный пользователь или null, если пользователь не найден.</returns>
    Task<User?> GetUserByEmail(string email);
    
    /// <summary>
    /// Метод, осуществляющий поиск пользователя по его имени пользователя.
    /// </summary>
    /// <param name="username">Имя пользователя.</param>
    /// <returns>Найденный пользователь или null, если пользователь не найден.</returns>
    Task<User?> GetUserByUsername(string username);
    
    /// <summary>
    /// Метод, осуществляющий добавление пользователя в базу данных.
    /// </summary>
    /// <param name="user">Добавляемый пользователь.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task CreateUser(User user);
}
