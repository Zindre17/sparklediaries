using backend.Responses;
using backend.Services;
using Dapper;

namespace backend.Stores;

public class HuntStore
{
    private readonly DatabaseService db;

    public HuntStore(DatabaseService db)
    {
        this.db = db;
    }

    internal async IAsyncEnumerable<Hunt> GetAll()
    {
        using var connection = db.GetConnection();
        var query = @"
                SELECT *
                FROM [Hunt] h
                LEFT JOIN [Encounters] e ON e.[HuntId] = h.[Id]
                WHERE [Complete] = 0
                GROUP BY h.[Id]
            ";
        foreach (var item in await connection.QueryAsync<StoredHunt>(query))
        {
            yield return item.ToHunt();
        };
    }

    public async Task<Hunt?> GetHunt(int id)
    {
        using var connection = db.GetConnection();
        var query = @$"
                SELECT h.*, SUM(e.[Count]) AS Encounters
                FROM [Hunt] h
                LEFT JOIN [Encounters] e ON e.[HuntId] = h.[Id]
                WHERE h.[Id] = {id}
                GROUP BY h.[Id]
            ";
        var result = await connection.QuerySingleOrDefaultAsync<StoredHunt>(query);
        return result?.ToHunt();
    }

    public async IAsyncEnumerable<Hunt> GetActive()
    {
        using var connection = db.GetConnection();
        var query = @"
                SELECT h.*, SUM(e.[Count]) AS Encounters 
                FROM [Hunt] h
                LEFT JOIN [Encounters] e ON e.[HuntId] = h.[Id]
                WHERE [Complete] = 0 GROUP BY h.[Id]
            ";
        foreach (var item in await connection.QueryAsync<StoredHunt>(query))
        {
            yield return item.ToHunt();
        }
    }

    public async Task<long> CreateHunt(string game, string type, string? target)
    {
        using var connection = db.GetConnection();
        var query = @"
                INSERT INTO [Hunt]([Game],[Type],[Target],[Complete]) 
                VALUES (@game, @type, @target, 0) returning [Id]
            ";
        return await connection.ExecuteScalarAsync<long>(query, new { game, type, target });
    }

    internal async Task<bool> AddEncounters(int id, int count)
    {
        var hunt = await GetHunt(id);
        if (hunt is null || hunt.Completed)
        {
            return false;
        }

        using var connection = db.GetConnection();
        var date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        var statement = @$"
                INSERT INTO [Encounters]([HuntId], [Count], [Date])
                VALUES ({id}, {count}, @date)
            ";
        var result = await connection.ExecuteAsync(statement, new { date });
        return result == 1;
    }

    public async Task<bool> CompleteHunt(int id)
    {
        using var connection = db.GetConnection();
        var statement = "UPDATE [Hunt] SET [Complete] = 1 WHERE [Id] = @id";
        var result = await connection.ExecuteAsync(statement, new { id });

        return result == 1;
    }
}

public record StoredHunt(int Id, string Game, string Type, string? Target, int Encounters, int Complete)
{
    public StoredHunt() : this(default, "", "", default, default, default) { }

    public Hunt ToHunt()
    {
        return new(Id, Game, Type, Target, Encounters, Complete == 1);
    }
}

