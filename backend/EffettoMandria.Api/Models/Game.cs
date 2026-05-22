namespace EffettoMandria.Api.Models;

public enum GamePhase
{
    Lobby,
    Answering,
    Voting,
    Results,
    Finished
}

public class Game
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public Guid DirectorUserId { get; set; }
    public GamePhase Phase { get; set; } = GamePhase.Lobby;
    public QuestionDto? CurrentQuestion { get; set; }
    public HashSet<int> UsedQuestionIds { get; set; } = [];
    public Dictionary<Guid, GamePlayer> Players { get; } = new();
    public List<RoundAnswer> CurrentAnswers { get; } = [];
    public List<RoundVote> CurrentVotes { get; } = [];
    public RoundResultDto? LastRoundResult { get; set; }
    /// <summary>Giocatore che porta la penalità visibile fino al prossimo evento penalità.</summary>
    public Guid? ActivePenaltyUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class GamePlayer
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = "";
    public int Score { get; set; }
    public int Penalties { get; set; }
    public bool HasSubmittedAnswer { get; set; }
    public bool HasVoted { get; set; }
}

public class RoundAnswer
{
    public Guid Id { get; set; }
    public Guid AuthorUserId { get; set; }
    public string Text { get; set; } = "";
    public bool IsHidden { get; set; } = true;
}

public class RoundVote
{
    public Guid VoterUserId { get; set; }
    public Guid AnswerId { get; set; }
}

public class RoundResultDto
{
    public bool IsTie { get; set; }
    public Guid? WinningAnswerId { get; set; }
    public string? WinningAnswerText { get; set; }
    public int WinningVoteCount { get; set; }
    public List<Guid> ScoredVoterIds { get; set; } = [];
    public Guid? PenaltyUserId { get; set; }
    public string? PenaltyReason { get; set; }
    public Dictionary<Guid, int> VoteCountsByAnswer { get; set; } = new();
}

public class GameStateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Phase { get; set; } = "";
    public QuestionDto? CurrentQuestion { get; set; }
    public List<PlayerStateDto> Players { get; set; } = [];
    public List<AnswerStateDto> Answers { get; set; } = [];
    public RoundResultDto? LastRoundResult { get; set; }
    public Guid? ActivePenaltyUserId { get; set; }
    public bool IsDirector { get; set; }
    public Guid? MyUserId { get; set; }
    public bool HasSubmittedAnswer { get; set; }
    public bool HasVoted { get; set; }
    public Guid? MyAnswerId { get; set; }
}

public class PlayerStateDto
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = "";
    public int Score { get; set; }
    public int Penalties { get; set; }
    public bool HasSubmittedAnswer { get; set; }
    public bool HasVoted { get; set; }
    public bool AnsweredThisRound { get; set; }
}

public class AnswerStateDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = "";
    public int VoteCount { get; set; }
    public bool IsMine { get; set; }
    public Guid? AuthorUserId { get; set; }
    public string? AuthorName { get; set; }
    public bool RevealAuthor { get; set; }
}
