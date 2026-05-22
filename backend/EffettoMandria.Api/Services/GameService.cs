using EffettoMandria.Api.Hubs;
using EffettoMandria.Api.Models;
using Microsoft.AspNetCore.SignalR;

namespace EffettoMandria.Api.Services;

public class GameService
{
    private readonly InMemoryStore _store;
    private readonly QuestionService _questions;
    private readonly ScoringService _scoring;
    private readonly IHubContext<GameHub> _hub;

    public GameService(
        InMemoryStore store,
        QuestionService questions,
        ScoringService scoring,
        IHubContext<GameHub> hub)
    {
        _store = store;
        _questions = questions;
        _scoring = scoring;
        _hub = hub;
    }

    public Game CreateGame(User director, string name)
    {
        EnsureNotInGame(director);
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Name = string.IsNullOrWhiteSpace(name) ? "Partita Mandria" : name.Trim(),
            DirectorUserId = director.Id
        };
        _store.Games[game.Id] = game;
        director.CurrentGameId = game.Id;
        return game;
    }

    public Game JoinGame(User user, Guid gameId)
    {
        if (!_store.Games.TryGetValue(gameId, out var game))
            throw new KeyNotFoundException("Partita non trovata.");
        if (game.Phase == GamePhase.Finished)
            throw new InvalidOperationException("Partita terminata.");
        if (user.CurrentGameId.HasValue && user.CurrentGameId != gameId)
            throw new InvalidOperationException("Sei già in un'altra partita.");
        if (user.Id == game.DirectorUserId)
            throw new InvalidOperationException("Il direttore di gioco non partecipa come giocatore.");
        AddPlayer(game, user);
        user.CurrentGameId = gameId;
        _ = NotifyGameAsync(gameId);
        return game;
    }

    public void LeaveGame(User user)
    {
        if (!user.CurrentGameId.HasValue) return;
        if (_store.Games.TryGetValue(user.CurrentGameId.Value, out var game))
        {
            game.Players.Remove(user.Id);
            if (game.DirectorUserId == user.Id && game.Players.Count > 0)
            {
                var next = game.Players.Values.First();
                game.DirectorUserId = next.UserId;
            }
            _ = NotifyGameAsync(game.Id);
        }
        user.CurrentGameId = null;
    }

    public GameStateDto GetState(Game game, User? viewer)
    {
        var isDirector = viewer?.Id == game.DirectorUserId;
        var revealAuthors = game.Phase == GamePhase.Results;

        var voteCounts = game.CurrentVotes
            .GroupBy(v => v.AnswerId)
            .ToDictionary(g => g.Key, g => g.Count());

        GamePlayer? myPlayer = null;
        if (viewer != null)
            game.Players.TryGetValue(viewer.Id, out myPlayer);

        var myAnswer = viewer != null
            ? game.CurrentAnswers.FirstOrDefault(a => a.AuthorUserId == viewer.Id)
            : null;

        return new GameStateDto
        {
            Id = game.Id,
            Name = game.Name,
            Phase = game.Phase.ToString().ToLowerInvariant(),
            CurrentQuestion = game.CurrentQuestion,
            Players = GetParticipatingPlayers(game)
                .OrderByDescending(p => p.Score)
                .ThenBy(p => p.DisplayName)
                .Select(p => new PlayerStateDto
                {
                    UserId = p.UserId,
                    DisplayName = p.DisplayName,
                    Score = p.Score,
                    Penalties = p.Penalties,
                    HasSubmittedAnswer = p.HasSubmittedAnswer,
                    HasVoted = p.HasVoted,
                    AnsweredThisRound = game.CurrentAnswers.Any(a => a.AuthorUserId == p.UserId)
                })
                .ToList(),
            Answers = BuildAnswerDtos(game, viewer, isDirector, revealAuthors, voteCounts),
            LastRoundResult = game.LastRoundResult,
            IsDirector = isDirector,
            MyUserId = viewer?.Id,
            HasSubmittedAnswer = myPlayer != null && (
                myPlayer.HasSubmittedAnswer ||
                game.CurrentAnswers.Any(a => a.AuthorUserId == viewer!.Id)),
            HasVoted = myPlayer?.HasVoted ?? false,
            MyAnswerId = myAnswer?.Id
        };
    }

    public async Task StartRoundAsync(Game game)
    {
        if (game.Phase != GamePhase.Lobby && game.Phase != GamePhase.Results)
            throw new InvalidOperationException("Impossibile avviare un turno in questa fase.");

        var question = _questions.PickRandom(game.UsedQuestionIds);
        if (question is null)
        {
            game.Phase = GamePhase.Finished;
            await NotifyGameAsync(game.Id);
            return;
        }

        game.UsedQuestionIds.Add(question.Id);
        game.CurrentQuestion = QuestionService.ToDto(question);
        game.CurrentAnswers.Clear();
        game.CurrentVotes.Clear();
        game.LastRoundResult = null;
        ResetRoundFlags(game);
        game.Phase = GamePhase.Answering;
        await NotifyGameAsync(game.Id);
    }

    public async Task SubmitAnswerAsync(Game game, User user, string text)
    {
        if (game.Phase != GamePhase.Answering)
            throw new InvalidOperationException("Non è il momento di rispondere.");
        if (user.Id == game.DirectorUserId)
            throw new InvalidOperationException("Il direttore di gioco non risponde alle domande.");
        if (!game.Players.ContainsKey(user.Id))
            throw new InvalidOperationException("Non sei in questa partita.");

        var trimmed = text.Trim();
        if (string.IsNullOrEmpty(trimmed))
            throw new ArgumentException("Risposta vuota.");

        if (game.CurrentQuestion?.Tipo == "multipla" &&
            game.CurrentQuestion.Opzioni?.Contains(trimmed) != true)
            throw new ArgumentException("Opzione non valida.");

        if (game.CurrentAnswers.Any(a => a.AuthorUserId == user.Id))
            throw new InvalidOperationException("Hai già risposto.");

        game.CurrentAnswers.Add(new RoundAnswer
        {
            Id = Guid.NewGuid(),
            AuthorUserId = user.Id,
            Text = trimmed
        });

        if (game.Players.TryGetValue(user.Id, out var player))
            player.HasSubmittedAnswer = true;

        if (AllPlayersSubmitted(game))
            await FinishRoundAsync(game);
        else
            await NotifyGameAsync(game.Id);
    }

    public async Task SubmitVoteAsync(Game game, User user, Guid answerId)
    {
        throw new InvalidOperationException("La votazione separata non è più usata: la risposta conta come voto.");
    }

    public async Task ForceEndVotingAsync(Game game) => await FinishRoundAsync(game);

    public async Task ForceEndTurnAsync(Game game)
    {
        if (game.Phase != GamePhase.Answering)
            throw new InvalidOperationException("Il turno non può essere chiuso in questa fase.");
        await FinishRoundAsync(game);
    }

    public async Task FinishRoundAsync(Game game)
    {
        if (game.Phase != GamePhase.Answering) return;
        foreach (var a in game.CurrentAnswers)
            a.IsHidden = false;
        var result = _scoring.CalculateFromAnswers(game);
        _scoring.ApplyRoundResult(game, result);
        game.LastRoundResult = result;
        game.Phase = GamePhase.Results;
        await NotifyGameAsync(game.Id);
    }

    public async Task ReturnToLobbyAsync(Game game)
    {
        game.CurrentQuestion = null;
        game.CurrentAnswers.Clear();
        game.CurrentVotes.Clear();
        game.LastRoundResult = null;
        ResetRoundFlags(game);
        game.Phase = GamePhase.Lobby;
        await NotifyGameAsync(game.Id);
    }

    public IReadOnlyList<GameSummaryDto> ListActiveGames() =>
        _store.Games.Values
            .Where(g => g.Phase != GamePhase.Finished)
            .OrderByDescending(g => g.CreatedAt)
            .Select(g => new GameSummaryDto
            {
                Id = g.Id,
                Name = g.Name,
                Phase = g.Phase.ToString().ToLowerInvariant(),
                PlayerCount = g.Players.Values.Count(p => p.UserId != g.DirectorUserId)
            })
            .ToList();

    private static IEnumerable<GamePlayer> GetParticipatingPlayers(Game game) =>
        game.Players.Values.Where(p => p.UserId != game.DirectorUserId);

    private static void AddPlayer(Game game, User user)
    {
        if (user.Id == game.DirectorUserId) return;
        if (game.Players.ContainsKey(user.Id)) return;
        game.Players[user.Id] = new GamePlayer
        {
            UserId = user.Id,
            DisplayName = user.DisplayName
        };
    }

    private static void EnsureNotInGame(User user)
    {
        if (user.CurrentGameId.HasValue)
            throw new InvalidOperationException("Sei già in una partita. Esci prima di crearne una nuova.");
    }

    private static void ResetRoundFlags(Game game)
    {
        foreach (var p in GetParticipatingPlayers(game))
        {
            p.HasSubmittedAnswer = false;
            p.HasVoted = false;
        }
    }

    private static bool AllPlayersSubmitted(Game game)
    {
        var players = GetParticipatingPlayers(game).ToList();
        return players.Count > 0 && players.All(p => p.HasSubmittedAnswer);
    }

    private static List<AnswerStateDto> BuildAnswerDtos(
        Game game,
        User? viewer,
        bool isDirector,
        bool revealAuthors,
        Dictionary<Guid, int> voteCounts)
    {
        if (game.Phase == GamePhase.Lobby || game.Phase == GamePhase.Answering)
        {
            if (!isDirector) return [];
            return game.CurrentAnswers.Select(a =>
            {
                game.Players.TryGetValue(a.AuthorUserId, out var author);
                return new AnswerStateDto
                {
                    Id = a.Id,
                    Text = a.Text,
                    AuthorUserId = a.AuthorUserId,
                    AuthorName = author?.DisplayName,
                    RevealAuthor = true
                };
            }).ToList();
        }

        var textCounts = game.CurrentAnswers
            .GroupBy(a => a.Text.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

        return game.CurrentAnswers.Select(a =>
        {
            game.Players.TryGetValue(a.AuthorUserId, out var author);
            var showAuthor = revealAuthors || isDirector;
            textCounts.TryGetValue(a.Text.Trim(), out var count);
            return new AnswerStateDto
            {
                Id = a.Id,
                Text = a.Text,
                VoteCount = count,
                IsMine = viewer?.Id == a.AuthorUserId,
                AuthorUserId = showAuthor ? a.AuthorUserId : null,
                AuthorName = showAuthor ? author?.DisplayName : null,
                RevealAuthor = showAuthor
            };
        }).ToList();
    }

    public async Task NotifyGameAsync(Guid gameId) =>
        await _hub.Clients.Group(gameId.ToString()).SendAsync("gameUpdated");
}

public class GameSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Phase { get; set; } = "";
    public int PlayerCount { get; set; }
}
