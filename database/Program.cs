using System.Reflection;
using DbUp;

var dbPath = args[0];

if (dbPath is null)
{
    Console.WriteLine("Please provide a path to db to patch.");
    return;
}

var connectionString = $"Data Source={dbPath}";

var upgrader = DeployChanges.To
    .SQLiteDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();

if (upgrader.IsUpgradeRequired())
{
    var result = upgrader.PerformUpgrade();
    if (result.Successful)
    {
        Console.WriteLine("Upgrade successful.");
    }
    else
    {
        Console.WriteLine("Upgarde failed.");
    }
}
else
{
    Console.WriteLine("Database is already up to date.");
}
