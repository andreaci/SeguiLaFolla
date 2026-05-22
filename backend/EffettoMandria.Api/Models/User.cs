namespace EffettoMandria.Api.Models;

public class User
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = "";
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    public bool IsGuest { get; set; }
    public Guid? CurrentGameId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AuthSession
{
    public Guid Token { get; set; }
    public Guid UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
}
