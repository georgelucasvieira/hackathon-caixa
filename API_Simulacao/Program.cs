using API_Simulacao.Config;
using API_Simulacao.DTOs.Simulacao;
using API_Simulacao.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

MigrationRunner.RunMigrations(
    builder.Configuration.GetConnectionString("DbProduto")!,
    "Migrations.Produto");

MigrationRunner.RunMigrations(
    builder.Configuration.GetConnectionString("DbSimulacao")!,
    "Migrations.Simulacao");

builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<SimulacaoRepository>();

DapperMappingConfig.Configure();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/simulacao", async (ProdutoRepository repo) =>
{
    var produtos = await repo.GetAllAsync();
    
    var response = new RetornoSimulacaoDTO();
    response.idSimulacao = 123123;
    //response.taxaJuros

    return Results.Ok(produtos);
})
.WithName("Simulacao")
.WithOpenApi();

app.Run();