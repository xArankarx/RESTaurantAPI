#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations.Schema;

namespace RESTaurantAPI.Authorization.Models;

/// <summary>
/// Класс, содержащий данные пользователя.
/// </summary>
public record User {
    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    [Column("id")]
    public int Id { get; set; }
    
    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [Column("username")]
    public string Username { get; set; }
    
    /// <summary>
    /// Email-адрес пользователя.
    /// </summary>
    [Column("email")]
    public string Email { get; set; }
    
    /// <summary>
    /// Хэш пароля пользователя.
    /// </summary>
    [Column("password_hash")]
    public string PasswordHash { get; set; }
    
    /// <summary>
    /// Роль пользователя.
    /// </summary>
    [Column("role")]
    public string Role { get; set; }
    
    /// <summary>
    /// Дата и время создания пользователя.
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Дата и время последнего обновления пользователя.
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
