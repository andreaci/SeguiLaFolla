using System.Text.Json;
using EffettoMandria.Api.Models;

namespace EffettoMandria.Api.Services;

public class QuestionService
{
    private readonly List<Question> _questions;
    private readonly Random _random = new();

    public QuestionService(IWebHostEnvironment env)
    {
        var path = Path.Combine(env.ContentRootPath, "Data", "questions.json");
        var json = File.ReadAllText(path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var raw = JsonSerializer.Deserialize<List<QuestionJson>>(json, options) ?? [];
        _questions = raw.Select(Map).ToList();
    }

    public IReadOnlyList<QuestionDto> GetAll() =>
        _questions.Select(ToDto).ToList();

    public Question? PickRandom(HashSet<int> usedIds)
    {
        var available = _questions.Where(q => !usedIds.Contains(q.Id)).ToList();
        if (available.Count == 0) return null;
        return available[_random.Next(available.Count)];
    }

    public static QuestionDto ToDto(Question q) => new()
    {
        Id = q.Id,
        Tipo = q.Tipo == QuestionType.Aperta ? "aperta" : "multipla",
        Domanda = q.Domanda,
        Opzioni = q.Opzioni
    };

    private static Question Map(QuestionJson j) => new()
    {
        Id = j.Id,
        Tipo = j.Tipo?.Equals("multipla", StringComparison.OrdinalIgnoreCase) == true
            ? QuestionType.Multipla
            : QuestionType.Aperta,
        Domanda = j.Domanda ?? "",
        Opzioni = j.Opzioni
    };

    private class QuestionJson
    {
        public int Id { get; set; }
        public string? Tipo { get; set; }
        public string? Domanda { get; set; }
        public List<string>? Opzioni { get; set; }

    }
}
