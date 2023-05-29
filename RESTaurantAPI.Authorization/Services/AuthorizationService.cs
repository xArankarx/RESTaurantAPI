using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using RESTaurantAPI.Authorization.Models;
using RESTaurantAPI.Authorization.Repositories;
using RESTaurantAPI.Authorization.Requests;

namespace RESTaurantAPI.Authorization.Services;

/// <summary>
/// Сервис, отвечающий за процессы, связанные с авторизацией пользователей.
/// </summary>
public class AuthorizationService {
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly JwtSettings _jwtSettings = new();

    /// <summary>
    /// Конструктор сервиса авторизации.
    /// </summary>
    /// <param name="userRepository">Репозиторий для работы с пользователями.</param>
    /// <param name="sessionRepository">Репозиторий для работы с сессиями.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public AuthorizationService(IUserRepository userRepository, ISessionRepository sessionRepository,
                                IConfiguration configuration) {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _jwtSettings.Key = configuration["JwtSettings:Key"]
                           ?? throw new ArgumentException("JWT key not found.");
        _jwtSettings.Issuer = configuration["JwtSettings:Issuer"]
                              ?? throw new ArgumentException("JWT issuer not found.");
        _jwtSettings.Audience = configuration["JwtSettings:Audience"]
                                ?? throw new ArgumentException("JWT audience not found.");
        var parseResult = int.TryParse(configuration["JwtSettings:ExpireDays"], out var expireDays);
        _jwtSettings.ExpireDays = parseResult ? expireDays : 30;
    }

    /// <summary>
    /// Метод, осуществляющий регистрацию пользователя.
    /// </summary>
    /// <param name="request">Запрос на регистрацию.</param>
    /// <returns>Зарегистрированный пользователь.</returns>
    /// <exception cref="ArgumentException">Данные пользователя некорректны.</exception>
    /// <exception cref="InvalidOperationException">Пользователь с таким email или username уже зарегистрирован.</exception>
    public async Task<User> RegisterUser(RegistrationRequest request) {
        // Проверка входных данных.
        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@')) {
            throw new ArgumentException("Invalid email address.");
        }

        // Проверка наличия пользователя с таким email.
        var existingUser = await _userRepository.GetUserByEmail(request.Email);
        if (existingUser != null) {
            throw new InvalidOperationException("Email address already registered.");
        }
        
        // Проверка наличия пользователя с таким username.
        existingUser = await _userRepository.GetUserByUsername(request.Username);
        if (existingUser != null) {
            throw new InvalidOperationException("Username already registered.");
        }

        // Хеширование пароля.
        var passwordHash = HashPassword(request.Password);

        // Создание нового пользователя.
        var user = new User {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        if (!VerifyRole(user.Role)) {
            throw new ArgumentException("Invalid role. Valid roles are: customer, chef, manager.");
        }

        // Сохранение пользователя в базе данных.
        await _userRepository.CreateUser(user);

        return user;
    }

    /// <summary>
    /// Метод, осуществляющий вход пользователя в систему.
    /// </summary>
    /// <param name="request">Запрос на вход.</param>
    /// <returns>JWT-токен.</returns>
    /// <exception cref="InvalidOperationException">Неверный email или пароль.</exception>
    public async Task<string> Login(LoginRequest request) {
        // Поиск пользователя по email.
        var user = await _userRepository.GetUserByEmail(request.Email);
        if (user == null) {
            throw new InvalidOperationException("Invalid email or password.");
        }

        // Проверка пароля.
        if (!VerifyPassword(request.Password, user.PasswordHash)) {
            throw new InvalidOperationException("Invalid email or password.");
        }

        // Генерация JWT-токена.
        var token = GenerateJwtToken(user);

        // Создание новой сессии.
        var session = new Session {
            UserId = user.Id,
            SessionToken = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1) // Set session expiration to 1 hour
        };

        // Сохранение сессии в базе данных.
        await _sessionRepository.CreateSession(session);

        return token;
    }

    /// <summary>
    /// Метод, осуществляющий получение информации о пользователе по токену.
    /// </summary>
    /// <param name="token">JWT-токен.</param>
    /// <returns>Информация о пользователе.</returns>
    /// <exception cref="InvalidOperationException">Неверный токен или пользователь не найден.</exception>
    public async Task<User> GetUserByToken(string token) {
        // Поиск сессии по токену.
        var session = await _sessionRepository.GetSessionByToken(token);
        if (session == null || session.ExpiresAt < DateTime.UtcNow) {
            throw new InvalidOperationException("Invalid token or session expired.");
        }

        // Поиск пользователя по id.
        var user = await _userRepository.GetUserById(session.UserId);
        if (user == null) {
            throw new InvalidOperationException("User not found.");
        }

        return user;
    }
    
    /// <summary>
    /// Метод, осуществляющий получение информации о пользователе по id.
    /// </summary>
    /// <param name="id">Id пользователя.</param>
    /// <returns>Информация о пользователе.</returns>
    /// <exception cref="InvalidOperationException">Пользователь не найден.</exception>
    public async Task<User> GetUserById(int id) {
        // Поиск пользователя по id.
        var user = await _userRepository.GetUserById(id);
        if (user == null) {
            throw new InvalidOperationException("User not found.");
        }

        return user;
    }

    /// <summary>
    /// Метод, осуществляющий хеширование пароля.
    /// </summary>
    /// <param name="password">Пароль.</param>
    /// <returns>Хеш пароля.</returns>
    private static string HashPassword(string password) {
        var salt = new byte[16];
        var gen = new Pkcs5S2ParametersGenerator();
        gen.Init(Encoding.ASCII.GetBytes(password), salt, 1000);
        var key = ((KeyParameter)gen.GenerateDerivedParameters("AES256", 256)).GetKey();
        return Convert.ToBase64String(key);
    }

    /// <summary>
    /// Метод, осуществляющий соответствие пароля и его хеша.
    /// </summary>
    /// <param name="password">Пароль.</param>
    /// <param name="passwordHash">Хеш пароля.</param>
    /// <returns>True, если пароль соответствует хешу, иначе false.</returns>
    private static bool VerifyPassword(string password, string passwordHash) {
        var salt = new byte[16];
        var gen = new Pkcs5S2ParametersGenerator();
        gen.Init(Encoding.ASCII.GetBytes(password), salt, 1000);
        var key = ((KeyParameter)gen.GenerateDerivedParameters("AES256", 256)).GetKey();
        return Convert.ToBase64String(key) == passwordHash;
    }
    
    /// <summary>
    /// Метод, осуществляющий проверку корректности роли.
    /// </summary>
    /// <param name="role">Роль.</param>
    /// <returns>True, если роль корректна, иначе false.</returns>
    private static bool VerifyRole(string role) {
        return role is "customer" or "chef" or "manager";
    }

    /// <summary>
    /// Метод, осуществляющий генерацию JWT-токена.
    /// </summary>
    /// <param name="user">Пользователь.</param>
    /// <returns>JWT-токен.</returns>
    private string GenerateJwtToken(User user) {
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(_jwtSettings.ExpireDays);

        var token = new JwtSecurityToken(issuer: _jwtSettings.Issuer,
                                         audience: _jwtSettings.Audience,
                                         claims: claims,
                                         expires: expires,
                                         signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
