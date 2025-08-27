using System.Data.Common;
using System.Reflection;
using API_Simulacao.DTOs.Simulacao;
using API_Simulacao.Enums;
using API_Simulacao.Middlewares;
using API_Simulacao.Models;
using API_Simulacao.Repositories;
using API_Simulacao.Services;
using DbUp;
using Microsoft.Data.SqlClient;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

string connStrDbSimulacao = builder.Configuration.GetConnectionString("DbSimulacao")!;
string connStrDbProdutos = builder.Configuration.GetConnectionString("DbProduto")!;

await SincronizarComBanco(() => new NpgsqlConnection(connStrDbSimulacao), "db_simulacao");
await SincronizarComBanco(() => new SqlConnection(connStrDbProdutos), "db_produto");

RunMigration(
    connStrDbSimulacao,
    filter: name => name.Contains("Migrations.Simulacao.", StringComparison.OrdinalIgnoreCase),
    logPrefix: "[db_simulacao]"
);

builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<SimulacaoRepository>();
builder.Services.AddScoped<TelemetriaRepository>();
builder.Services.AddScoped<SimulacaoService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<TelemetryMiddleware>();

app.MapPost("/simulacoes", async (ProdutoRepository produtoRepo, SimulacaoService simulacaoService, EntradaSimulacaoDTO request) =>
{
    var response = await simulacaoService.RealizarSimulacao(request);
    return Results.Ok(response);
})
.WithOpenApi();

app.MapGet("/simulacoes", async (SimulacaoRepository simulacaoRepo, int? pagina, int? limite) =>
{
    var simulacoes = await simulacaoRepo.GetAllByTipoPaginatedAsync(pagina ?? 1, limite ?? 10, TipoSimulacao.PRICE);
    return Results.Ok(simulacoes);
})
.WithOpenApi();

app.MapGet("/simulacoes/relatorio", async (SimulacaoRepository simulacaoRepo, DateTime dataReferencia, int codigoProduto) =>
{
    var relatorioDiario = await simulacaoRepo.GetAllByDataProdutoTipoAsync(dataReferencia, codigoProduto, TipoSimulacao.PRICE);

    var retorno = new RetornoRelatorioDiarioDTO
    {
        dataReferencia = dataReferencia,
        simulacoes = relatorioDiario
    };

    return Results.Ok(retorno);
})
.WithOpenApi();

app.MapGet("/telemetria/metricas", async (TelemetriaRepository telemetriaRepo, DateTime dataReferencia) =>
{
    var metricas = await telemetriaRepo.ObterMetricasAsync(dataReferencia);
    var retorno = new TelemetriaDTO
    {
        dataReferencia = dataReferencia,
        listaEndpoints = metricas
    };
    return Results.Ok(retorno);
})
.WithOpenApi();

app.Run();

static async Task SincronizarComBanco(Func<DbConnection> connFactory, string name, int retries = 30)
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

static void RunMigration(string connectionString, Func<string, bool> filter, string logPrefix)
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