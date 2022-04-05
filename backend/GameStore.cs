using backend.Responses;
using backend.Services;
using Dapper;

namespace backend.Stores;

public class GameStore
{
    private readonly DatabaseService db;

    public GameStore(DatabaseService db)
    {
        this.db = db;
    }

    public async IAsyncEnumerable<Game> All()
    {
        using var connection = db.GetConnection();
        var query = "SELECT * FROM [Game]";
        foreach (var record in await connection.QueryAsync<PersistedGame>(query))
        {
            yield return record.ToGame();
        }
    }
    public async Task<long?> GetId(string game)
    {
        using var connection = db.GetConnection();
        var query = "SELECT [Id] FROM [Game] WHERE UPPER([Name]) = UPPER(@game)";
        return await connection.ExecuteScalarAsync<long?>(query, new { game });
    }

    private record PersistedGame(long Id, string Name, long Generation)
    {
        public Game ToGame() => new(Name, Generation);
    }
}
