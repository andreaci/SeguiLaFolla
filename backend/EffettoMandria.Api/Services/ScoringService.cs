using EffettoMandria.Api.Models;

namespace EffettoMandria.Api.Services;

public class ScoringService
{
    /// <summary>
    /// Ogni risposta inviata conta come un voto per quel testo (nessuna fase di votazione separata).
    /// Pareggio al primo posto: nessun punto. Penalità solo se esiste una sola risposta distinta con un solo voto.
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

        if (byText.Count == 1 && byText[0].Count == 1)
        {
            var lone = byText[0].Answers[0];
            result.PenaltyUserId = lone.AuthorUserId;
            result.PenaltyReason = "Risposta solitaria: penalità!";
        }

        var maxCount = byText.Max(x => x.Count);
        var winningGroups = byText.Where(x => x.Count == maxCount).ToList();

        if (winningGroups.Count > 1)
        {
            result.IsTie = true;
            return result;
        }

        var winner = winningGroups[0];
        result.WinningAnswerText = winner.Text;
        result.WinningVoteCount = winner.Count;
        result.WinningAnswerId = winner.Answers[0].Id;
        result.ScoredVoterIds = winner.Answers
            .Select(a => a.AuthorUserId)
            .Distinct()
            .ToList();

        return result;
    }

    public void ApplyRoundResult(Game game, RoundResultDto result)
    {
        if (!result.IsTie)
        {
            foreach (var playerId in result.ScoredVoterIds)
            {
                if (game.Players.TryGetValue(playerId, out var player))
                    player.Score++;
            }
        }

        if (result.PenaltyUserId.HasValue)
        {
            game.ActivePenaltyUserId = result.PenaltyUserId;
            if (game.Players.TryGetValue(result.PenaltyUserId.Value, out var penalized))
                penalized.Penalties++;
        }
    }
}
