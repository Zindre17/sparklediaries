namespace backend.Responses;

public record Hunt(
    int Id,
    string Game,
    string Type,
    string? Target,
    int Encounters,
    bool Completed
);
