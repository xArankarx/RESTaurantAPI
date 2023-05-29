#pragma warning disable CS8618

namespace RESTaurantAPI.Authorization.Models;

/// <summary>
/// Класс, содержащий настройки для работы с JWT-токенами.
/// </summary>
public record JwtSettings {
    /// <summary>
    /// Секретный ключ.
    /// </summary>
    public string Key { get; set; }
    
    /// <summary>
    /// Эмитент токена.
    /// </summary>
    public string Issuer { get; set; }
    
    /// <summary>
    /// Аудитория токена.
    /// </summary>
    public string Audience { get; set; }
    
    /// <summary>
    /// Длительность жизни токена в днях.
    /// </summary>
    public int ExpireDays { get; set; }
}
