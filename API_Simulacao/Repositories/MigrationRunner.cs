using System.Reflection;
using DbUp;

namespace API_Simulacao.Repositories;

public static class MigrationRunner
{
    public static void RunMigrations(string connectionString, string filterNamespace)
    {
        var result = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                Assembly.GetExecutingAssembly(),
                s => s.StartsWith(filterNamespace))
            .LogToConsole()
            .Build()
            .PerformUpgrade();

        if (!result.Successful)
        {
            throw result.Error;
        }
    }
}