using System.Security.Cryptography;
using System.Text;
using EffettoMandria.Api.Models;

namespace EffettoMandria.Api.Services;

public class AuthService(InMemoryStore store)
{
    public (User user, Guid token) Register(RegisterRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            throw new ArgumentException("Username e password obbligatori.");
        if (store.UsernameIndex.ContainsKey(req.Username))
            throw new InvalidOperationException("Username già in uso.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = req.Username.Trim(),
            PasswordHash = HashPassword(req.Password),
            DisplayName = string.IsNullOrWhiteSpace(req.DisplayName) ? req.Username.Trim() : req.DisplayName.Trim(),
            IsGuest = false
        };
        store.Users[user.Id] = user;
        store.UsernameIndex[user.Username] = user.Id;
        return (user, CreateSession(user.Id));
    }

    public (User user, Guid token) Login(LoginRequest req)
    {
        if (!store.UsernameIndex.TryGetValue(req.Username, out var userId))
            throw new UnauthorizedAccessException("Credenziali non valide.");
        var user = store.Users[userId];
        if (user.PasswordHash != HashPassword(req.Password))
            throw new UnauthorizedAccessException("Credenziali non valide.");
        return (user, CreateSession(user.Id));
    }

    public (User user, Guid token) CreateGuest(GuestRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.DisplayName))
            throw new ArgumentException("Nome obbligatorio.");
        var user = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = req.DisplayName.Trim(),
            IsGuest = true
        };
        store.Users[user.Id] = user;
        return (user, CreateSession(user.Id));
    }

    public User? GetUserFromToken(Guid? token)
    {
        if (token is null || token == Guid.Empty) return null;
        if (!store.Sessions.TryGetValue(token.Value, out var session)) return null;
        if (session.ExpiresAt < DateTime.UtcNow)
        {
            store.Sessions.TryRemove(token.Value, out _);
            return null;
        }
        return store.Users.GetValueOrDefault(session.UserId);
    }

    private Guid CreateSession(Guid userId)
    {
        var token = Guid.NewGuid();
        store.Sessions[token] = new AuthSession
        {
            Token = token,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddHours(12)
        };
        return token;
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
