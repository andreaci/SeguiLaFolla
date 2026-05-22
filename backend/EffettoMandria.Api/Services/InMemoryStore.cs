using System.Collections.Concurrent;
using EffettoMandria.Api.Models;

namespace EffettoMandria.Api.Services;

public class InMemoryStore
{
    public ConcurrentDictionary<Guid, User> Users { get; } = new();
    public ConcurrentDictionary<Guid, AuthSession> Sessions { get; } = new();
    public ConcurrentDictionary<Guid, Game> Games { get; } = new();
    public ConcurrentDictionary<string, Guid> UsernameIndex { get; } = new(StringComparer.OrdinalIgnoreCase);
}
