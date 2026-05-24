using System.Text.Json;
using System.Text.Json.Serialization;
using EffettoMandria.Api.Models;

namespace EffettoMandria.Api.Services;

public class QuestionService
{
    private readonly List<Question> _questions;
    private readonly Random _random = new();
    private static JsonSerializerOptions? json_options;

    public QuestionService(IWebHostEnvironment env)
    {
        var path = Path.Combine(env.ContentRootPath, "Data", "questions.json");
        var json = File.ReadAllText(path);
        json_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var raw = JsonSerializer.Deserialize<List<QuestionJson>>(json, json_options) ?? [];
        _questions = [.. raw.Select(Map)];
    }

    public IReadOnlyList<QuestionDto> GetAll() =>
        [.. _questions.Select(ToDto)];

    public IReadOnlyList<QuestionCategoryDto> GetCategories(HashSet<int> usedIds)
    {
        return [.. _questions
            .Where(q => !string.IsNullOrWhiteSpace(q.Categoria))
            .GroupBy(q => q.Categoria.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => new QuestionCategoryDto
            {
                Id = g.Key,
                Label = FormatCategoryLabel(g.Key),
                Available = g.Count(q => !usedIds.Contains(q.Id))
            })
            .OrderBy(c => c.Label, StringComparer.OrdinalIgnoreCase)];
    }

    public Question? PickRandom(HashSet<int> usedIds, string? category = null)
    {
        List<Question> available = [.. _questions.Where(q => !usedIds.Contains(q.Id))];
        if (!string.IsNullOrWhiteSpace(category))
        {
            available = [.. available.Where(q => string.Equals(q.Categoria.Trim(), category.Trim(), StringComparison.OrdinalIgnoreCase))];
        }

        if (available.Count == 0) return null;
        return available[_random.Next(available.Count)];
    }

    public static QuestionDto ToDto(Question q) => new()
    {
        Id = q.Id,
        Tipo = q.Tipo == QuestionType.Aperta ? "aperta" : "multipla",
        Categoria = q.Categoria,
        Domanda = q.Domanda,
        Opzioni = q.Opzioni
    };

    private static string FormatCategoryLabel(string key)
    {
        if (string.IsNullOrEmpty(key)) return key;
        return char.ToUpperInvariant(key[0]) + key[1..];
    }

    private static Question Map(QuestionJson j) => new()
    {
        Id = j.Id,
        Tipo = j.Tipo?.Equals("multipla", StringComparison.OrdinalIgnoreCase) == true
            ? QuestionType.Multipla
            : QuestionType.Aperta,
        Categoria = (j.Categoria ?? "").Trim(),
        Domanda = j.Domanda ?? "",
        Opzioni = j.Opzioni
    };

    private class QuestionJson
    {
        public int Id { get; set; }
        public string? Tipo { get; set; }
        public string? Categoria { get; set; }
        public string? Domanda { get; set; }

        [JsonPropertyName("opzioni")]
        public List<string>? Opzioni { get; set; }
    }
}
