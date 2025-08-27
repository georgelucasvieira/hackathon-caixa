using System.Data.Common;
using System.Reflection;
using DbUp;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace API_Simulacao.Config;

public static class DatabaseExtensions
{
    public static async Task ConfigurarDatabases(this WebApplicationBuilder builder)
    {
        string connStrDbSimulacao = builder.Configuration.GetConnectionString("DbSimulacao")!;
        string connStrDbProdutos = builder.Configuration.GetConnectionString("DbProduto")!;

        await SincronizarComBanco(() => new NpgsqlConnection(connStrDbSimulacao), "db_simulacao");
        await SincronizarComBanco(() => new SqlConnection(connStrDbProdutos), "db_produto");

        RunMigration(
            connStrDbSimulacao,
            filter: name => name.Contains("Migrations.Simulacao.", StringComparison.OrdinalIgnoreCase),
            logPrefix: "[db_simulacao]"
        );
    }

    private static async Task SincronizarComBanco(Func<DbConnection> connFactory, string name, int retries = 30)
    {
        Console.WriteLine($"Waiting {name}...");
        for (var i = 1; i <= retries; i++)
        {
            try
            {
                await using var conn = connFactory();
                await conn.OpenAsync();
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT 1";
                await cmd.ExecuteScalarAsync();
                Console.WriteLine($"[{name}] pronto.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Aguardando {name}... tent {i}/{retries} ({ex.Message})");
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }
        throw new Exception($"Timeout aguardando {name}");
    }

    private static void RunMigration(string connectionString, Func<string, bool> filter, string logPrefix)
    {
        Console.WriteLine($"{logPrefix} Rodando migrations...");
        var upgrader =
            DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), filter)
                .LogToConsole()
                .Build();

        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw result.Error!;
        Console.WriteLine($"{logPrefix} Migrations finalizado");
    }
}
