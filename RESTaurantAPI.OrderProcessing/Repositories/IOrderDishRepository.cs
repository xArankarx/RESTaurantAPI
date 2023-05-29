using RESTaurantAPI.OrderProcessing.Models;

namespace RESTaurantAPI.OrderProcessing.Repositories; 

/// <summary>
/// Интерфейс репозитория для работы с блюдами заказа.
/// </summary>
public interface IOrderDishRepository {
    /// <summary>
    /// Метод, осуществляющий добавление нового блюда заказа в базу данных.
    /// </summary>
    /// <param name="orderDish">Добавляемое блюдо заказа.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task CreateOrderDish(OrderDish orderDish);

    /// <summary>
    /// Метод, осуществляющий удаление блюда заказа из базы данных.
    /// </summary>
    /// <param name="orderDish">Удаляемое блюдо заказа.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task DeleteOrderDish(OrderDish orderDish);

    /// <summary>
    /// Метод, осуществляющий получение всех блюд заказа по идентификатору заказа.
    /// </summary>
    /// <param name="orderId">Идентификатор заказа.</param>
    /// <returns>Найденные блюда заказа.</returns>
    Task<IEnumerable<OrderDish>> GetOrderDishesByOrderId(int orderId);
    
    /// <summary>
    /// Метод, осуществляющий получение всех блюд заказа по идентификатору блюда.
    /// </summary>
    /// <param name="dishId">Идентификатор блюда.</param>
    /// <returns>Найденные блюда заказа.</returns>
    Task<IEnumerable<OrderDish>> GetOrderDishesByDishId(int dishId);
}
