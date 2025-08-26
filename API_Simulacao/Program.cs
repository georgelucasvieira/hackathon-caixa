using System.Reflection;
using API_Simulacao.Config;
using API_Simulacao.DTOs.Simulacao;
using API_Simulacao.Repositories;
using DbUp;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

string connStrDbSimulacao = builder.Configuration.GetConnectionString("DbSimulacao")!;

RunMigration(
    connStrDbSimulacao,
    filter: name => name.Contains("Migrations.Simulacao.", StringComparison.OrdinalIgnoreCase),
    logPrefix: "[db_simulacao/]"
);

builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<SimulacaoRepository>();

DapperMappingConfig.Configure();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/simular", async (ProdutoRepository produtoRepo, EntradaSimulacaoDTO request) =>
{
    var produtos = await produtoRepo.GetByValorEPrazoAsync(request.valorDesejado, request.prazo);
    
    var response = new RetornoSimulacaoDTO();
    response.idSimulacao = 123123;
    //response.taxaJuros

    return Results.Ok(produtos);
})
.WithName("FazerSimulacao")
.WithOpenApi();

app.MapGet("/simulacao/listar", async (SimulacaoRepository simulacaoRepo, int? pagina, int? limite) =>
{
    var simulacoes = await simulacaoRepo.GetAllPaginatedAsync(pagina ?? 1, limite ?? 10);
    return Results.Ok(simulacoes);

})
.WithName("ObterSimulacoes")
.WithOpenApi();

//bonus/opcional
app.MapGet("/simulacao/detalhe/{id}", async (int id) =>
{
    return Results.Ok();
})
.WithName("ObterDetalheSimulacao")
.WithOpenApi();

app.MapGet("/simulacao/relatorioDiario", async (DateTime dataReferencia) =>
{
    Results.Ok();
})
.WithName("ObterRelatorio")
.WithOpenApi();

app.MapGet("/healthcheck/telemetria", async (DateTime dataReferencia) =>
{
    Results.Ok();
})
.WithName("Telemetria")
.WithOpenApi();



app.Run();

static async Task WaitForSqlAsync(string connStr, string name, int retries = 30)
{
    Console.WriteLine($"Waiting {name}...");
    for (var i = 1; i <= retries; i++)
    {
        try
        {
            await using var conn = new SqlConnection(connStr);
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