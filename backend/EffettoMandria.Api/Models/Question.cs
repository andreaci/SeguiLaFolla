namespace EffettoMandria.Api.Models;

public enum QuestionType
{
    Aperta,
    Multipla
}

public class Question
{
    public int Id { get; set; }
    public QuestionType Tipo { get; set; }
    public string Categoria { get; set; } = "";
    public string Domanda { get; set; } = "";
    public List<string>? Opzioni { get; set; }
}

public class QuestionDto
{
    public int Id { get; set; }
    public string Tipo { get; set; } = "";
    public string Categoria { get; set; } = "";
    public string Domanda { get; set; } = "";
    public List<string>? Opzioni { get; set; }
}

public class QuestionCategoryDto
{
    public string Id { get; set; } = "";
    public string Label { get; set; } = "";
    public int Available { get; set; }
}
