using System.Configuration;
using System.Data.SQLite;
using Microsoft.Extensions.Options;

namespace backend.Services;

public class DatabaseService
{
    private readonly string connectionString;

    public DatabaseService(IOptionsSnapshot<DatabaseOptions> snapshot)
    {
        connectionString = snapshot.Value.ConnectionString ??
            throw new ConfigurationErrorsException("Missing DatabaseOptions__ConnectionString option");
    }
    public SQLiteConnection GetConnection()
    {
        var connection = new SQLiteConnection(connectionString);
        connection.Open();
        return connection;
    }
}
