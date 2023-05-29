using RESTaurantAPI.OrderProcessing.Models;

namespace RESTaurantAPI.OrderProcessing.Repositories; 

/// <summary>
/// Интерфейс репозитория для работы с блюдами.
/// </summary>
public interface IDishRepository {
    /// <summary>
    /// Метод, осуществляющий поиск блюда по его идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор блюда.</param>
    /// <returns>Найденное блюдо.</returns>
    Task<Dish?> GetDishById(int id);
    
    /// <summary>
    /// Метод, осуществляющий добавление нового блюда в базу данных.
    /// </summary>
    /// <param name="dish">Добавляемое блюдо.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task CreateDish(Dish dish);
    
    /// <summary>
    /// Метод, осуществляющий обновление информации о блюде в базе данных.
    /// </summary>
    /// <param name="dish">Обновляемое блюдо.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task UpdateDish(Dish dish);
    
    /// <summary>
    /// Метод, осуществляющий удаление блюда из базы данных.
    /// </summary>
    /// <param name="dish">Удаляемое блюдо.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task DeleteDish(Dish dish);
    
    /// <summary>
    /// Метод, осуществляющий получение всех блюд из базы данных.
    /// </summary>
    /// <returns>Найденные блюда.</returns>
    Task<IEnumerable<Dish>> GetAllDishes();
    
    /// <summary>
    /// Метод, осуществляющий поиск блюд по названию.
    /// </summary>
    /// <param name="name">Название блюда.</param>
    /// <returns>Найденные блюда.</returns>
    Task<IEnumerable<Dish>> GetDishesByName(string name);
    
    /// <summary>
    /// Метод, осуществляющий поиск блюд по доступности.
    /// </summary>
    /// <param name="isAvailable">Доступность блюда.</param>
    /// <returns>Найденные блюда.</returns>
    Task<IEnumerable<Dish>> GetDishesByAvailability(bool isAvailable);
}
