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
        var query = GetHuntQuery("WHERE [Complete] = 0");
        foreach (var item in await connection.QueryAsync<StoredHunt>(query))
        {
            yield return item.ToHunt();
        };
    }

    private static string GetHuntQuery(string? whereClause)
    {
        return @$"
            SELECT 
                h.[Id],
                g.[Name] as Game,
                t.[Type],
                h.[Target],
                COALESCE(SUM(e.[Count]), 0) AS Encounters,
                h.[Complete]
            FROM [Hunt] h
            INNER JOIN [Game] g ON g.[Id] = h.[Game]
            INNER JOIN [HuntType] t on t.[Id] = h.[Type]
            LEFT JOIN [Encounter] e ON e.[HuntId] = h.[Id]
            {whereClause ?? ""}
            GROUP BY h.[Id], g.[Name], t.[Type], h.[Target], h.[Complete]
        ";
    }

    internal async IAsyncEnumerable<Hunt> GetCompleted()
    {
        using var connection = db.GetConnection();
        var query = GetHuntQuery("WHERE h.[Complete] = 1");
        foreach (var record in await connection.QueryAsync<StoredHunt>(query))
        {
            yield return record!.ToHunt();
        }
    }

    internal async IAsyncEnumerable<string> GetHuntTypes()
    {
        using var connection = db.GetConnection();
        var query = "SELECT [Type] FROM [HuntType]";
        foreach (var record in await connection.QueryAsync<string>(query))
        {
            yield return record;
        }
    }

    public async Task<Hunt?> GetHunt(int id)
    {
        using var connection = db.GetConnection();
        var query = GetHuntQuery($"WHERE h.[Id] = {id}");
        var result = await connection.QuerySingleOrDefaultAsync<StoredHunt>(query);
        return result?.ToHunt();
    }

    public async IAsyncEnumerable<Hunt> GetActive()
    {
        using var connection = db.GetConnection();
        var query = GetHuntQuery("WHERE h.[Complete] = 0");
        foreach (var item in await connection.QueryAsync<StoredHunt>(query))
        {
            yield return item.ToHunt();
        }
    }

    internal async Task<long?> GetHuntTypeId(string type)
    {
        using var connection = db.GetConnection();
        var query = "SELECT [Id] FROM [HuntType] WHERE UPPER([Type]) = UPPER(@type)";
        return await connection.ExecuteScalarAsync<long?>(query, new { type });
    }

    public async Task<long> CreateHunt(long game, long type, string? target)
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
                INSERT INTO [Encounter]([HuntId], [Count], [DateTime])
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

    private record StoredHunt(long Id, string Game, string Type, string? Target, long Encounters, long Complete)
    {
        public StoredHunt() : this(default, string.Empty, string.Empty, default, default, default) { }

        public Hunt ToHunt()
        {
            return new(Id, Game, Type, Target, Encounters, Complete == 1);
        }
    }
}


