#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations.Schema;

namespace RESTaurantAPI.Authorization.Models;

/// <summary>
/// Класс, содержащий данные сессии.
/// </summary>
public record Session {
    /// <summary>
    /// Идентификатор сессии.
    /// </summary>
    [Column("id")]
    public int Id { get; set; }
    
    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    [Column("user_id")]
    public int UserId { get; set; }
    
    /// <summary>
    /// JWT-токен сессии.
    /// </summary>
    [Column("session_token")]
    public string SessionToken { get; set; }
    
    /// <summary>
    /// Дата и время истечения сессии.
    /// </summary>
    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }
}
