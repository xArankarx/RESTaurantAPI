#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;

namespace RESTaurantAPI.Authorization.Requests;

/// <summary>
/// Модель запроса на авторизацию.
/// </summary>
public record LoginRequest {
    /// <summary>
    /// Электронная почта пользователя.
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(100, MinimumLength = 5)]
    public string Email { get; init; }
    
    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; init; }
}
