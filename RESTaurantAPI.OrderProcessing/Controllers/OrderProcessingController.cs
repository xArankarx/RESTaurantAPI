using Microsoft.AspNetCore.Mvc;
using RESTaurantAPI.OrderProcessing.Models;
using RESTaurantAPI.OrderProcessing.Requests;
using RESTaurantAPI.OrderProcessing.Services;

namespace RESTaurantAPI.OrderProcessing.Controllers;

/// <summary>
/// Контроллер, отвечающий за логику взаимодействия с запросами к API обработки заказов.
/// </summary>
[ApiController]
[Route("api/order-processing")]
public class OrderProcessingController : ControllerBase {
    private readonly OrderProcessingService _orderProcessingService;

    /// <summary>
    /// Конструктор контроллера.
    /// </summary>
    /// <param name="orderProcessingService">Сервис обработки заказов.</param>
    public OrderProcessingController(OrderProcessingService orderProcessingService) {
        _orderProcessingService = orderProcessingService;
    }

    /// <summary>
    /// Метод, осуществляющий создание заказа.
    /// </summary>
    /// <param name="request">Запрос на создание заказа.</param>
    /// <returns>Результат создания заказа.</returns>
    [HttpPost("order/create")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request) {
        Order order;
        try {
            order = await _orderProcessingService.CreateOrder(request);
        }
        catch (ArgumentException e) {
            return BadRequest(e.Message);
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok(order);
    }
    
    /// <summary>
    /// Метод, осуществляющий обновление статуса заказа. Доступен только менеджеру.
    /// </summary>
    /// <param name="role">Роль пользователя.</param>
    /// <param name="id">Идентификатор заказа.</param>
    /// <param name="status">Обновленный статус заказа.</param>
    /// <returns>Результат обновления статуса заказа.</returns>
    [HttpPut("order/{id:int}/update/{status}")]
    public async Task<IActionResult> UpdateOrderStatus([FromHeader] string role, int id, string status) {
        if (role != "manager") {
            return Unauthorized("You are not authorized to perform this action.");
        }
        
        Order order;
        try {
            order = await _orderProcessingService.UpdateOrderStatus(id, status);
        }
        catch (ArgumentException e) {
            return BadRequest(e.Message);
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok(order);
    }

    /// <summary>
    /// Метод, осуществляющий обработку всех необработанных заказов.
    /// </summary>
    /// <returns>Результат обработки всех необработанных заказов.</returns>
    [HttpPost("order/all/process")]
    public async Task<IActionResult> ProcessOrders() {
        try {
            await _orderProcessingService.ProcessOrders();
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok("All orders have been processed.");
    }

    /// <summary>
    /// Метод, осуществляющий получение заказа по его идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <returns>Результат получения заказа.</returns>
    [HttpGet("order/{id:int}")]
    public async Task<IActionResult> GetOrderById(int id) {
        Order order;
        try {
            order = await _orderProcessingService.GetOrderById(id);
        }
        catch (ArgumentException e) {
            return NotFound(e.Message);
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok(order);
    }

    /// <summary>
    /// Метод, осуществляющий получение всех заказов.
    /// </summary>
    /// <returns>Результат получения всех заказов.</returns>
    [HttpGet("order/all")]
    public async Task<IActionResult> GetAllOrders() {
        IEnumerable<Order> orders;
        try {
            orders = await _orderProcessingService.GetAllOrders();
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok(orders);
    }
    
    /// <summary>
    /// Метод, осуществляющий получение всех заказов по идентификатору пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Результат получения всех заказов по идентификатору пользователя.</returns>
    [HttpGet("order/user/{userId:int}")]
    public async Task<IActionResult> GetOrdersByUserId(int userId) {
        IEnumerable<Order> orders;
        try {
            orders = await _orderProcessingService.GetOrdersByUserId(userId);
        }
        catch (ArgumentException e) {
            return NotFound(e.Message);
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok(orders);
    }
    
    /// <summary>
    /// Метод, осуществляющий получение всех заказов по статусу.
    /// </summary>
    /// <param name="status">Статус заказа.</param>
    /// <returns>Результат получения всех заказов по статусу.</returns>
    [HttpGet("order/status/{status}")]
    public async Task<IActionResult> GetOrdersByStatus(string status) {
        IEnumerable<Order> orders;
        try {
            orders = await _orderProcessingService.GetOrdersByStatus(status);
        }
        catch (ArgumentException e) {
            return NotFound(e.Message);
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok(orders);
    }
    
    /// <summary>
    /// Метод, осуществляющий создание блюда. Доступен только менеджеру.
    /// </summary>
    /// <param name="role">Роль пользователя.</param>
    /// <param name="request">Запрос на создание блюда.</param>
    /// <returns>Результат создания блюда.</returns>
    [HttpPost("dish/create")]
    public async Task<IActionResult> CreateDish([FromHeader] string role, [FromBody] CreateDishRequest request) {
        if (role != "manager") {
            return Unauthorized("You are not authorized to interact with dishes.");
        }

        Dish dish;
        try {
            dish = await _orderProcessingService.CreateDish(request);
        }
        catch (ArgumentException e) {
            return BadRequest(e.Message);
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok(dish);
    }
    
    /// <summary>
    /// Метод, осуществляющий обновление блюда. Доступен только менеджеру.
    /// </summary>
    /// <param name="role">Роль пользователя.</param>
    /// <param name="id">Идентификатор блюда.</param>
    /// <param name="request">Запрос на обновление блюда.</param>
    /// <returns>Результат обновления блюда.</returns>
    [HttpPut("dish/update/{id:int}")]
    public async Task<IActionResult> UpdateDish([FromHeader] string role, int id, [FromBody] CreateDishRequest request) {
        if (role != "manager") {
            return Unauthorized("You are not authorized to interact with dishes.");
        }

        Dish dish;
        try {
            dish = await _orderProcessingService.UpdateDish(id, request);
        }
        catch (ArgumentException e) {
            return BadRequest(e.Message);
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok(dish);
    }
    
    /// <summary>
    /// Метод, осуществляющий удаление блюда. Доступен только менеджеру.
    /// </summary>
    /// <param name="role">Роль пользователя.</param>
    /// <param name="id">Идентификатор блюда.</param>
    /// <returns>Результат удаления блюда.</returns>
    [HttpDelete("dish/delete/{id:int}")]
    public async Task<IActionResult> DeleteDish([FromHeader] string role, int id) {
        if (role != "manager") {
            return Unauthorized("You are not authorized to interact with dishes.");
        }

        try {
            await _orderProcessingService.DeleteDish(id);
        }
        catch (ArgumentException e) {
            return NotFound(e.Message);
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok("Dish deleted.");
    }
    
    /// <summary>
    /// Метод, осуществляющий получение блюда по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор блюда.</param>
    /// <returns>Результат получения блюда по идентификатору.</returns>
    [HttpGet("dish/{id:int}")]
    public async Task<IActionResult> GetDishById(int id) {
        Dish dish;
        try {
            dish = await _orderProcessingService.GetDishById(id);
        }
        catch (ArgumentException e) {
            return NotFound(e.Message);
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok(dish);
    }
    
    /// <summary>
    /// Метод, осуществляющий получение всех блюд.
    /// </summary>
    /// <returns>Результат получения всех блюд.</returns>
    [HttpGet("dish/all")]
    public async Task<IActionResult> GetAllDishes() {
        IEnumerable<Dish> dishes;
        try {
            dishes = await _orderProcessingService.GetAllDishes();
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok(dishes);
    }

    /// <summary>
    /// Метод, осуществляющий получение блюд по названию.
    /// </summary>
    /// <param name="name">Название блюда.</param>
    /// <returns>Результат получения блюд по названию.</returns>
    [HttpGet("dish/name/{name}")]
    public async Task<IActionResult> GetDishesByName(string name) {
        IEnumerable<Dish> dishes;
        try {
            dishes = await _orderProcessingService.GetDishesByName(name);
        }
        catch (ArgumentException e) {
            return NotFound(e.Message);
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok(dishes);
    }

    /// <summary>
    /// Метод, осуществляющий получение меню (списка доступных блюд).
    /// </summary>
    /// <returns>Результат получения меню.</returns>
    [HttpGet("menu")]
    public async Task<IActionResult> GetAvailableDishes() {
        IEnumerable<Dish> dishes;
        try {
            dishes = await _orderProcessingService.GetDishesByAvailability(true);
        } 
        catch (ArgumentException e) {
            return NotFound(e.Message);
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok(dishes);
    }
}
