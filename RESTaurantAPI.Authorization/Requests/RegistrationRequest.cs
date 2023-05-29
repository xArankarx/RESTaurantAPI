#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;

namespace RESTaurantAPI.Authorization.Requests;

/// <summary>
/// Модель запроса на регистрацию пользователя.
/// </summary>
public record RegistrationRequest {
    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Username { get; init; }
    
    /// <summary>
    /// Адрес электронной почты.
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
    
    /// <summary>
    /// Роль пользователя.
    /// </summary>
    [Required]
    [StringLength(10)]
    public string Role { get; init; }
}
