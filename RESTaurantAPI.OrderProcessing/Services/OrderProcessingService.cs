#pragma warning disable CS4014

using RESTaurantAPI.OrderProcessing.Models;
using RESTaurantAPI.OrderProcessing.Repositories;
using RESTaurantAPI.OrderProcessing.Requests;

namespace RESTaurantAPI.OrderProcessing.Services;

/// <summary>
/// Сервис для обработки заказов.
/// </summary>
public class OrderProcessingService {
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDishRepository _orderDishRepository;
    private readonly IDishRepository _dishRepository;

    /// <summary>
    /// Конструктор сервиса.
    /// </summary>
    /// <param name="orderRepository">Репозиторий для работы с заказами.</param>
    /// <param name="orderDishRepository">Репозиторий для работы с блюдами заказа.</param>
    /// <param name="dishRepository">Репозиторий для работы с блюдами.</param>
    public OrderProcessingService(IOrderRepository orderRepository, IOrderDishRepository orderDishRepository,
                                  IDishRepository dishRepository) {
        _orderRepository = orderRepository;
        _orderDishRepository = orderDishRepository;
        _dishRepository = dishRepository;
    }

    /// <summary>
    /// Метод, осуществляющий создание заказа.
    /// </summary>
    /// <param name="request">Запрос на создание заказа.</param>
    /// <returns>Созданный заказ.</returns>
    /// <exception cref="ArgumentException">Какое-либо блюдо в заказе не существует или не имеет достаточного количества в наличии.</exception>
    public async Task<Order> CreateOrder(CreateOrderRequest request) {
        // Проверка корректности статуса заказа.
        if (!VerifyStatus(request.Status)) {
            throw new ArgumentException($"Status '{request.Status}' is not valid.");
        }

        // Создание заказа.
        var order = new Order {
            UserId = request.UserId,
            Status = "Pending",
            SpecialRequests = request.SpecialRequests,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        
        await _orderRepository.CreateOrder(order);

        // Привязка блюд к заказу.
        foreach (var orderDish in request.OrderDishes) {
            var dish = await _dishRepository.GetDishById(orderDish.DishId);

            if (dish == null) {
                await _orderRepository.DeleteOrder(order);
                throw new ArgumentException($"Dish with id '{orderDish.DishId}' does not exist.");
            }
            if (dish.Quantity < orderDish.Quantity) {
                await _orderRepository.DeleteOrder(order);
                throw new ArgumentException($"Dish with id '{orderDish.DishId}' does not have enough quantity in stock.");
            }

            var newOrderDish = new OrderDish {
                OrderId = order.Id,
                DishId = dish.Id,
                Quantity = orderDish.Quantity,
                Price = dish.Price * orderDish.Quantity
            };

            await _orderDishRepository.CreateOrderDish(newOrderDish);
            
            dish.Quantity -= orderDish.Quantity;
            await _dishRepository.UpdateDish(dish);
            
            order.OrderDishes.Add(newOrderDish);
        }

        return order;
    }
    
    /// <summary>
    /// Метод, осуществляющий обновление статуса заказа.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <param name="status">Обновленный статус заказа.</param>
    /// <returns>Обновленный заказ.</returns>
    /// <exception cref="ArgumentException">Заказ с указанным идентификатором не существует или статус некорректен.</exception>
    public async Task<Order> UpdateOrderStatus(int id, string status) {
        // Проверка корректности статуса заказа.
        if (!VerifyStatus(status)) {
            throw new ArgumentException($"Status '{status}' is not valid.");
        }

        // Получение заказа по идентификатору.
        var order = await _orderRepository.GetOrderById(id);

        if (order == null) {
            throw new ArgumentException($"Order with id '{id}' does not exist.");
        }
        
        // Получение блюд к заказу для дальнейшего возврата.
        try {
            order.OrderDishes = (await GetOrderDishesByOrderId(id)).ToList();
        }
        catch (Exception) {
            order.OrderDishes = new List<OrderDish>();
        }

        // Запрет на отмену заказа, если он уже отменен или завершен.
        if (order.Status is not "Cancelled" and not "Completed" && status == "Cancelled") {
            // Возвращаем зарезервированные блюда в наличие.
            foreach (var orderDish in order.OrderDishes) {
                var dish = await _dishRepository.GetDishById(orderDish.DishId);

                if (dish == null) {
                    continue;
                }
                
                dish.Quantity += orderDish.Quantity;
                await _dishRepository.UpdateDish(dish);
            }
        }
        
        // Запрет на повторное совершение заказа, если он уже завершен или отменен, при отсутствии блюд в наличии.
        if (order.Status is "Cancelled" or "Completed" && status is "Pending" or "Processing") {
            // Проверка наличия блюд в наличии.
            foreach (var orderDish in order.OrderDishes) {
                var dish = await _dishRepository.GetDishById(orderDish.DishId);

                if (dish == null) {
                    throw new ArgumentException($"Dish with id '{orderDish.DishId}' does not exist.");
                }
                if (dish.Quantity < orderDish.Quantity) {
                    throw new ArgumentException($"Dish with id '{orderDish.DishId}' does not have enough quantity in stock.");
                }
                
                // Резервирование блюд.
                dish.Quantity -= orderDish.Quantity;
                await _dishRepository.UpdateDish(dish);
            }
        }

        // Обновление статуса заказа.
        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        await _orderRepository.UpdateOrder(order);

        return order;
    }

    /// <summary>
    /// Метод, осуществляющий получение заказа по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <returns>Заказ с указанным идентификатором.</returns>
    /// <exception cref="ArgumentException">Заказ с указанным идентификатором не существует.</exception>
    public async Task<Order> GetOrderById(int id) {
        // Получение заказа по идентификатору.
        var order = await _orderRepository.GetOrderById(id);

        if (order == null) {
            throw new ArgumentException($"Order with id '{id}' does not exist.");
        }
        
        // Получение блюд к заказу для дальнейшего возврата.
        try {
            order.OrderDishes = (await GetOrderDishesByOrderId(id)).ToList();
        }
        catch (Exception) {
            order.OrderDishes = new List<OrderDish>();
        }

        return order;
    }

    /// <summary>
    /// Метод, осуществляющий получение всех заказов.
    /// </summary>
    /// <returns>Список всех заказов.</returns>
    public async Task<IEnumerable<Order>> GetAllOrders() {
        // Получение всех заказов.
        var orders = (await _orderRepository.GetAllOrders()).ToList();
        
        // Получение блюд к заказам для дальнейшего возврата.
        foreach (var order in orders) {
            try {
                order.OrderDishes = (await GetOrderDishesByOrderId(order.Id)).ToList();
            }
            catch (Exception) {
                order.OrderDishes = new List<OrderDish>();
            }
        }

        return orders;
    }

    /// <summary>
    /// Метод, осуществляющий получение всех заказов по идентификатору пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Список всех заказов пользователя.</returns>
    /// <exception cref="ArgumentException">Пользователь с указанным идентификатором не существует или у него нет заказов.</exception>
    public async Task<IEnumerable<Order>> GetOrdersByUserId(int userId) {
        // Получение всех заказов пользователя.
        var orders = (await _orderRepository.GetOrdersByUserId(userId)).ToList();

        if (orders.Count == 0) {
            throw new ArgumentException($"No orders exist for user with id '{userId}'.");
        }
        
        // Получение блюд к заказам для дальнейшего возврата.
        foreach (var order in orders) {
            try {
                order.OrderDishes = (await GetOrderDishesByOrderId(order.Id)).ToList();
            }
            catch (Exception) {
                order.OrderDishes = new List<OrderDish>();
            }
        }

        return orders;
    }

    /// <summary>
    /// Метод, осуществляющий получение всех заказов по статусу.
    /// </summary>
    /// <param name="status">Статус заказа.</param>
    /// <returns>Список всех заказов с указанным статусом.</returns>
    /// <exception cref="ArgumentException">Заказов с указанным статусом не существует.</exception>
    public async Task<IEnumerable<Order>> GetOrdersByStatus(string status) {
        // Поиск заказов по статусу.
        var orders = (await _orderRepository.GetOrdersByStatus(status)).ToList();

        if (orders.Count == 0) {
            throw new ArgumentException($"No orders exist with status '{status}'.");
        }
        
        // Получение блюд к заказам для дальнейшего возврата.
        foreach (var order in orders) {
            try {
                order.OrderDishes = (await GetOrderDishesByOrderId(order.Id)).ToList();
            }
            catch (Exception) {
                order.OrderDishes = new List<OrderDish>();
            }
        }

        return orders;
    }

    /// <summary>
    /// Метод, осуществляющий обработку всех необработанных заказов.
    /// </summary>
    /// <exception cref="ArgumentException">Все заказы уже обработаны.</exception>
    public async Task ProcessOrders() {
        // Получение всех необработанных заказов.
        var orders = (await _orderRepository.GetOrdersByStatus("Pending"))
                     .Union(await _orderRepository.GetOrdersByStatus("Processing")).ToList();
        
        // Получение блюд к заказам для дальнейшего возврата.
        foreach (var order in orders) {
            try {
                order.OrderDishes = (await GetOrderDishesByOrderId(order.Id)).ToList();
            }
            catch (Exception) {
                order.OrderDishes = new List<OrderDish>();
            }
        }

        if (orders.Count == 0) {
            throw new ArgumentException("All orders are already processed.");
        }

        // Обработка заказов.
        foreach (var order in orders) {
            await _orderRepository.ProcessOrder(order);
        }
    }

    /// <summary>
    /// Метод, осуществляющий создание блюда.
    /// </summary>
    /// <param name="request">Запрос на создание блюда.</param>
    /// <returns>Созданное блюдо.</returns>
    public async Task<Dish> CreateDish(CreateDishRequest request) {
        // Создание блюда.
        var dish = new Dish {
            Name = request.Name,
            Description = request.Description,
            Quantity = request.Quantity,
            Price = request.Price,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dishRepository.CreateDish(dish);

        return dish;
    }

    /// <summary>
    /// Метод, осуществляющий обновление блюда.
    /// </summary>
    /// <param name="id">Идентификатор блюда.</param>
    /// <param name="request">Запрос на обновление блюда.</param>
    /// <returns>Обновленное блюдо.</returns>
    /// <exception cref="ArgumentException">Блюдо с указанным идентификатором не существует.</exception>
    public async Task<Dish> UpdateDish(int id, CreateDishRequest request) {
        // Получение блюда по идентификатору.
        var dish = await _dishRepository.GetDishById(id);

        if (dish == null) {
            throw new ArgumentException($"Dish with id '{id}' does not exist.");
        }

        // Обновление блюда.
        dish.Name = request.Name;
        dish.Description = request.Description;
        dish.Quantity = request.Quantity;
        dish.Price = request.Price;
        dish.UpdatedAt = DateTime.UtcNow;

        await _dishRepository.UpdateDish(dish);

        return dish;
    }

    /// <summary>
    /// Метод, осуществляющий удаление блюда.
    /// </summary>
    /// <param name="id">Идентификатор блюда.</param>
    /// <exception cref="ArgumentException">Блюдо с указанным идентификатором не существует.</exception>
    public async Task DeleteDish(int id) {
        // Получение блюда по идентификатору.
        var dish = await _dishRepository.GetDishById(id);

        if (dish == null) {
            throw new ArgumentException($"Dish with id '{id}' does not exist.");
        }
        
        // Удаление всех заказанных блюд с указанным идентификатором.
        List<OrderDish> orderDishes;
        try {
            orderDishes = (await GetOrderDishesByDishId(id)).ToList();
        }
        catch (Exception) {
            orderDishes = new List<OrderDish>();
        }
        
        foreach (var orderDish in orderDishes) {
            await _orderDishRepository.DeleteOrderDish(orderDish);
        }

        // Удаление блюда.
        await _dishRepository.DeleteDish(dish);
    }

    /// <summary>
    /// Метод, осуществляющий получение блюда по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор блюда.</param>
    /// <returns>Блюдо с указанным идентификатором.</returns>
    /// <exception cref="ArgumentException">Блюдо с указанным идентификатором не существует.</exception>
    public async Task<Dish> GetDishById(int id) {
        // Получение блюда по идентификатору.
        var dish = await _dishRepository.GetDishById(id);

        if (dish == null) {
            throw new ArgumentException($"Dish with id '{id}' does not exist.");
        }

        return dish;
    }

    /// <summary>
    /// Метод, осуществляющий получение всех блюд.
    /// </summary>
    /// <returns>Список всех блюд.</returns>
    public async Task<IEnumerable<Dish>> GetAllDishes() {
        return await _dishRepository.GetAllDishes();
    }

    /// <summary>
    /// Метод, осуществляющий получение блюд по названию.
    /// </summary>
    /// <param name="name">Название блюда.</param>
    /// <returns>Список блюд с указанным названием.</returns>
    /// <exception cref="ArgumentException">Блюд с указанным названием не существует.</exception>
    public async Task<IEnumerable<Dish>> GetDishesByName(string name) {
        // Получение блюд по названию.
        var dishes = (await _dishRepository.GetDishesByName(name)).ToList();

        if (dishes.Count == 0) {
            throw new ArgumentException($"No dishes with name '{name}' exist.");
        }

        return dishes;
    }

    /// <summary>
    /// Метод, осуществляющий получение блюд по наличию (получение меню).
    /// </summary>
    /// <param name="available">Статус наличия блюда.</param>
    /// <returns>Список блюд с указанным статусом наличия.</returns>
    /// <exception cref="ArgumentException">Блюд с указанным статусом наличия не существует.</exception>
    public async Task<IEnumerable<Dish>> GetDishesByAvailability(bool available) {
        var dishes = (await _dishRepository.GetDishesByAvailability(available)).ToList();

        if (dishes.Count == 0) {
            throw new ArgumentException($"No dishes with availability '{available}' exist.");
        }

        return dishes;
    }

    /// <summary>
    /// Метод, осуществляющий получение блюд в заказе по идентификатору заказа.
    /// </summary>
    /// <param name="orderId">Идентификатор заказа.</param>
    /// <returns>Список блюд в заказе с указанным идентификатором заказа.</returns>
    /// <exception cref="ArgumentException">Блюд в заказе с указанным идентификатором заказа не существует.</exception>
    private async Task<IEnumerable<OrderDish>> GetOrderDishesByOrderId(int orderId) {
        // Получение блюд в заказе по идентификатору заказа.
        var orderDishes = (await _orderDishRepository.GetOrderDishesByOrderId(orderId)).ToList();

        if (!orderDishes.Any()) {
            throw new ArgumentException($"No order dishes with order id '{orderId}' exist.");
        }

        return orderDishes;
    }

    /// <summary>
    /// Метод, осуществляющий получение блюд в заказах по идентификатору блюда.
    /// </summary>
    /// <param name="dishId">Идентификатор блюда.</param>
    /// <returns>Список блюд в заказах с указанным идентификатором блюда.</returns>
    /// <exception cref="ArgumentException">Блюд в заказах с указанным идентификатором блюда не существует.</exception>
    private async Task<IEnumerable<OrderDish>> GetOrderDishesByDishId(int dishId) {
        // Получение блюд в заказах по идентификатору блюда.
        var orderDishes = (await _orderDishRepository.GetOrderDishesByDishId(dishId)).ToList();

        if (!orderDishes.Any()) {
            throw new ArgumentException($"No order dishes with dish id '{dishId}' exist.");
        }

        return orderDishes;
    }

    /// <summary>
    /// Метод, осуществляющий проверку статуса заказа на корректность.
    /// </summary>
    /// <param name="status">Статус заказа.</param>
    /// <returns>Результат проверки.</returns>
    private static bool VerifyStatus(string status) {
        return status is "Pending" or "Processing" or "Completed" or "Cancelled";
    }
}
