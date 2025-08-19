using API_Simulacao.Repositories;
using API_Simulacao.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<SimulacaoRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/simulacao", async (ProdutoRepository repo) =>
{
    var produtos = await repo.GetAllAsync();
    return Results.Ok(produtos);
})
.WithName("produtos")
.WithOpenApi();

app.Run();