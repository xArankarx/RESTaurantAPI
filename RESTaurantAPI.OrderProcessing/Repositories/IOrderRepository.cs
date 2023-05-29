using RESTaurantAPI.OrderProcessing.Models;

namespace RESTaurantAPI.OrderProcessing.Repositories; 

/// <summary>
/// Интерфейс репозитория для работы с заказами.
/// </summary>
public interface IOrderRepository {
    /// <summary>
    /// Метод, осуществляющий поиск заказа по его идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <returns>Найденный заказ или null, если заказ не найден.</returns>
    Task<Order?> GetOrderById(int id);
    
    /// <summary>
    /// Метод, осуществляющий добавление нового заказа в базу данных.
    /// </summary>
    /// <param name="order">Добавляемый заказ.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task CreateOrder(Order order);
    
    /// <summary>
    /// Метод, осуществляющий обновление данных заказа в базе данных.
    /// </summary>
    /// <param name="order">Обновляемый заказ.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task UpdateOrder(Order order);
    
    /// <summary>
    /// Метод, осуществляющий удаление заказа из базы данных.
    /// </summary>
    /// <param name="order">Удаляемый заказ.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task DeleteOrder(Order order);
    
    /// <summary>
    /// Метод, осуществляющий обработку заказа.
    /// </summary>
    /// <param name="order">Обрабатываемый заказ.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task ProcessOrder(Order order);
    
    /// <summary>
    /// Метод, осуществляющий получение всех заказов из базы данных.
    /// </summary>
    /// <returns>Все заказы.</returns>
    Task<IEnumerable<Order>> GetAllOrders();
    
    /// <summary>
    /// Метод, осуществляющий получение всех заказов, сделанных пользователем с указанным идентификатором.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Найденные заказы.</returns>
    Task<IEnumerable<Order>> GetOrdersByUserId(int userId);
    
    /// <summary>
    /// Метод, осуществляющий получение всех заказов с указанным статусом.
    /// </summary>
    /// <param name="status">Статус заказа.</param>
    /// <returns>Найденные заказы.</returns>
    Task<IEnumerable<Order>> GetOrdersByStatus(string status);
}
