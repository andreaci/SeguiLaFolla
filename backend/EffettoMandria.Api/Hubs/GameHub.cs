using EffettoMandria.Api.Services;
using Microsoft.AspNetCore.SignalR;

namespace EffettoMandria.Api.Hubs;

public class GameHub : Hub
{
    public async Task JoinGameGroup(string gameId)
    {
        if (Guid.TryParse(gameId, out _))
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
    }

    public async Task LeaveGameGroup(string gameId)
    {
        if (Guid.TryParse(gameId, out _))
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
    }
}
