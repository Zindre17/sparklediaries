namespace backend.Requests;

public record NewHunt(
    string? Game,
    string? Type,
    string? Target
);
