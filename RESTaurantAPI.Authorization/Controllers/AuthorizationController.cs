using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTaurantAPI.Authorization.Models;
using RESTaurantAPI.Authorization.Requests;
using RESTaurantAPI.Authorization.Services;

namespace RESTaurantAPI.Authorization.Controllers;

/// <summary>
/// Контроллер, отвечающий за логику взаимодействия с запросами к API авторизации.
/// </summary>
[ApiController]
[Route("api/authorization")]
public class AuthorizationController : ControllerBase {
    private readonly AuthorizationService _authorizationService;

    /// <summary>
    /// Конструктор контроллера.
    /// </summary>
    /// <param name="authorizationService">Сервис авторизации.</param>
    public AuthorizationController(AuthorizationService authorizationService) {
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Метод, осуществляющий регистрацию пользователя.
    /// </summary>
    /// <param name="request">Запрос на регистрацию.</param>
    /// <returns>Результат регистрации.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest request) {
        try {
            await _authorizationService.RegisterUser(request);
        }
        catch (ArgumentException e) {
            return BadRequest(e.Message);
        }
        catch (InvalidOperationException e) {
            return Conflict(e.Message);
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok("Registration successful.");
    }

    /// <summary>
    /// Метод, осуществляющий вход пользователя в систему.
    /// </summary>
    /// <param name="request">Запрос на вход.</param>
    /// <returns>Результат входа.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request) {
        string token;
        try {
            token = await _authorizationService.Login(request);
        }
        catch (InvalidOperationException e) {
            return NotFound(e.Message);
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok(new { token });
    }

    /// <summary>
    /// Метод, осуществляющий получение информации о пользователе. Требует авторизации.
    /// </summary>
    /// <returns>Информация о пользователе.</returns>
    [HttpGet("user"), Authorize]
    public async Task<IActionResult> GetUser() {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Split(' ')[1];
        User user;
        try {
            user = await _authorizationService.GetUserByToken(token);
        }
        catch (InvalidOperationException e) {
            return NotFound(e.Message);
        }
        catch (Exception e) {
            return StatusCode(500, e.Message);
        }

        return Ok(new { user.Id, user.Username, user.Email, user.Role });
    }
}
