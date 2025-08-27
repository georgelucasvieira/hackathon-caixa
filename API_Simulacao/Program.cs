using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using API_Simulacao.Config;
using API_Simulacao.DTOs;
using API_Simulacao.DTOs.Simulacao;
using API_Simulacao.Enums;
using API_Simulacao.Middlewares;
using API_Simulacao.Models;
using API_Simulacao.Repositories;
using API_Simulacao.Services;
using API_Simulacao.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

await builder.ConfigurarDatabases();

builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<SimulacaoRepository>();
builder.Services.AddScoped<TelemetriaRepository>();
builder.Services.AddScoped<SimulacaoService>();
builder.Services.AddSingleton<EventHubSDK>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<TelemetryMiddleware>();

app.MapPost("/simulacoes", async (ProdutoRepository produtoRepo, SimulacaoService simulacaoService, EntradaSimulacaoDTO request) =>
{
    var validationResults = new List<ValidationResult>();
    var context = new ValidationContext(request);

    if (!Validator.TryValidateObject(request, context, validationResults, true))
    {
        return Results.BadRequest(validationResults.Select(v => new
        {
            Campo = v.MemberNames.FirstOrDefault(),
            Erro = v.ErrorMessage
        }));
    }

    var response = await simulacaoService.RealizarSimulacao(request);
    return Results.Ok(response);
})
.AddEndpointFilter<ValidationFilter<EntradaSimulacaoDTO>>()
.WithOpenApi();

app.MapGet("/simulacoes", async (SimulacaoRepository simulacaoRepo, int? pagina, int? limite) =>
{
    var simulacoes = await simulacaoRepo.GetAllByTipoPaginatedAsync(pagina ?? 1, limite ?? 10, TipoSimulacao.PRICE);
    return Results.Ok(simulacoes);
})
.WithOpenApi();

app.MapGet("/simulacoes/relatorio", async (SimulacaoRepository simulacaoRepo,  DateTime? dataReferencia, int codigoProduto) =>
{
    var _dataReferencia = dataReferencia ?? DateTime.UtcNow.Date;
    var relatorioDiario = await simulacaoRepo.GetAllByDataProdutoTipoAsync(_dataReferencia, codigoProduto, TipoSimulacao.PRICE);

    var retorno = new RetornoRelatorioDiarioDTO
    {
        dataReferencia = _dataReferencia,
        simulacoes = relatorioDiario
    };

    return Results.Ok(retorno);
})
.WithOpenApi();

app.MapGet("/telemetria/metricas", async (TelemetriaRepository telemetriaRepo, DateTime? dataReferencia) =>
{
    var _dataReferencia = dataReferencia ?? DateTime.UtcNow.Date;
    var metricas = await telemetriaRepo.ObterMetricasAsync(_dataReferencia);
    var retorno = new RetornoTelemetriaDTO
    {
        dataReferencia = _dataReferencia,
        listaEndpoints = metricas
    };
    return Results.Ok(retorno);
})
.WithOpenApi();

app.Run();