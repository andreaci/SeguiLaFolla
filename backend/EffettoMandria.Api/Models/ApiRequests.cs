namespace EffettoMandria.Api.Models;

public record RegisterRequest(string Username, string Password, string DisplayName);
public record LoginRequest(string Username, string Password);
public record GuestRequest(string DisplayName);
public record CreateGameRequest(string Name);
public record SubmitAnswerRequest(string Text);
public record SubmitVoteRequest(Guid AnswerId);
public record StartRoundRequest(string? Categoria);
