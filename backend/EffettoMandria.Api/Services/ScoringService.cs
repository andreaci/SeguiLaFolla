using EffettoMandria.Api.Models;

namespace EffettoMandria.Api.Services;

public class ScoringService
{
    /// <summary>
    /// Ogni risposta inviata conta come un voto per quel testo (nessuna fase di votazione separata).
    /// </summary>
    public RoundResultDto CalculateFromAnswers(Game game)
    {
        var answers = game.CurrentAnswers;
        var result = new RoundResultDto();

        if (answers.Count == 0)
        {
            result.PenaltyReason = "Nessuna risposta.";
            return result;
        }

        var byText = answers
            .GroupBy(a => a.Text.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => new
            {
                Text = g.Key,
                Count = g.Count(),
                Answers = g.ToList()
            })
            .ToList();

        result.VoteCountsByAnswer = answers.ToDictionary(
            a => a.Id,
            a => byText.First(g => g.Text.Equals(a.Text.Trim(), StringComparison.OrdinalIgnoreCase)).Count);

        if (answers.Count == 1)
        {
            var lone = answers[0];
            result.PenaltyUserId = lone.AuthorUserId;
            result.WinningAnswerId = lone.Id;
            result.WinningAnswerText = lone.Text;
            result.WinningVoteCount = 1;
            result.PenaltyReason = "Unica risposta: vince la penalità.";
            return result;
        }

        var maxCount = byText.Max(x => x.Count);
        var winningGroups = byText.Where(x => x.Count == maxCount).ToList();
        var primary = winningGroups[0];

        result.WinningAnswerText = primary.Text;
        result.WinningVoteCount = maxCount;
        result.WinningAnswerId = primary.Answers[0].Id;
        result.ScoredVoterIds = winningGroups
            .SelectMany(g => g.Answers)
            .Select(a => a.AuthorUserId)
            .Distinct()
            .ToList();

        return result;
    }

    public void ApplyRoundResult(Game game, RoundResultDto result)
    {
        foreach (var playerId in result.ScoredVoterIds)
        {
            if (game.Players.TryGetValue(playerId, out var player))
                player.Score++;
        }

        if (result.PenaltyUserId.HasValue &&
            game.Players.TryGetValue(result.PenaltyUserId.Value, out var penalized))
        {
            penalized.Penalties++;
        }
    }
}
