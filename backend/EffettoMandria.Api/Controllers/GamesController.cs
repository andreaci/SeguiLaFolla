using EffettoMandria.Api.Models;
using EffettoMandria.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EffettoMandria.Api.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly InMemoryStore _store;
    private readonly AuthService _auth;
    private readonly GameService _games;

    public GamesController(InMemoryStore store, AuthService auth, GameService games)
    {
        _store = store;
        _auth = auth;
        _games = games;
    }

    [HttpGet]
    public IActionResult List() => Ok(_games.ListActiveGames());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGameRequest req)
    {
        var user = TryGetUser();
        if (user is null) return Unauthorized();
        try
        {
            var game = _games.CreateGame(user, req.Name);
            await _games.NotifyGameAsync(game.Id);
            return Ok(_games.GetState(game, user));
        }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id)
    {
        var user = HttpContext.GetCurrentUser(_auth);
        if (!_store.Games.TryGetValue(id, out var game))
            return NotFound(new { error = "Partita non trovata." });
        return Ok(_games.GetState(game, user));
    }

    [HttpPost("{id:guid}/join")]
    public async Task<IActionResult> Join(Guid id)
    {
        var user = TryGetUser();
        if (user is null) return Unauthorized();
        if (!_store.Games.ContainsKey(id)) return NotFound(new { error = "Partita non trovata." });
        try
        {
            var game = _games.JoinGame(user, id);
            await _games.NotifyGameAsync(game.Id);
            return Ok(_games.GetState(game, user));
        }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpPost("{id:guid}/leave")]
    public IActionResult Leave(Guid id)
    {
        var user = TryGetUser();
        if (user is null) return Unauthorized();
        if (user.CurrentGameId != id)
            return BadRequest(new { error = "Non sei in questa partita." });
        _games.LeaveGame(user);
        return Ok();
    }

    [HttpPost("{id:guid}/round/start")]
    public async Task<IActionResult> StartRound(Guid id)
    {
        var user = TryGetUser();
        if (user is null) return Unauthorized();
        if (!TryGetGame(id, out var game)) return NotFound(new { error = "Partita non trovata." });
        if (game.DirectorUserId != user.Id) return Forbid();
        try
        {
            await _games.StartRoundAsync(game);
            return Ok(_games.GetState(game, user));
        }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpPost("{id:guid}/answer")]
    public async Task<IActionResult> Answer(Guid id, [FromBody] SubmitAnswerRequest req)
    {
        var user = TryGetUser();
        if (user is null) return Unauthorized();
        if (!TryGetGame(id, out var game)) return NotFound(new { error = "Partita non trovata." });
        try
        {
            await _games.SubmitAnswerAsync(game, user, req.Text);
            return Ok(_games.GetState(game, user));
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpPost("{id:guid}/vote")]
    public async Task<IActionResult> Vote(Guid id, [FromBody] SubmitVoteRequest req)
    {
        var user = TryGetUser();
        if (user is null) return Unauthorized();
        if (!TryGetGame(id, out var game)) return NotFound(new { error = "Partita non trovata." });
        try
        {
            await _games.SubmitVoteAsync(game, user, req.AnswerId);
            return Ok(_games.GetState(game, user));
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpPost("{id:guid}/round/end-voting")]
    public async Task<IActionResult> EndVoting(Guid id)
    {
        var user = TryGetUser();
        if (user is null) return Unauthorized();
        if (!TryGetGame(id, out var game)) return NotFound(new { error = "Partita non trovata." });
        if (game.DirectorUserId != user.Id) return Forbid();
        await _games.FinishRoundAsync(game);
        return Ok(_games.GetState(game, user));
    }

    [HttpPost("{id:guid}/round/force-end")]
    public async Task<IActionResult> ForceEndTurn(Guid id)
    {
        var user = TryGetUser();
        if (user is null) return Unauthorized();
        if (!TryGetGame(id, out var game)) return NotFound(new { error = "Partita non trovata." });
        if (game.DirectorUserId != user.Id) return Forbid();
        try
        {
            await _games.ForceEndTurnAsync(game);
            return Ok(_games.GetState(game, user));
        }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpPost("{id:guid}/round/to-lobby")]
    public async Task<IActionResult> ToLobby(Guid id)
    {
        var user = TryGetUser();
        if (user is null) return Unauthorized();
        if (!TryGetGame(id, out var game)) return NotFound(new { error = "Partita non trovata." });
        if (game.DirectorUserId != user.Id) return Forbid();
        await _games.ReturnToLobbyAsync(game);
        return Ok(_games.GetState(game, user));
    }

    private User? TryGetUser() => HttpContext.GetCurrentUser(_auth);

    private bool TryGetGame(Guid id, out Game game) => _store.Games.TryGetValue(id, out game!);
}
