namespace backend.Responses;

public record Hunt(
    long Id,
    string Game,
    string Type,
    string? Target,
    long Encounters,
    bool Completed
);
