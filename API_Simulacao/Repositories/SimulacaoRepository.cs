using API_Simulacao.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace API_Simulacao.Repositories;

public class SimulacaoRepository
{
    private readonly IDbConnection  _db;
    private readonly IConfiguration _configuration;
    public SimulacaoRepository(IConfiguration configuration) 
    { 
        _configuration = configuration;
        _db = new SqlConnection(_configuration.GetConnectionString("SimulacaoDb"));
    }


    public async Task<List<Simulacao>> GetAllPaginatedAsync(int pageNumber, int pageSize)
    {
        var sql = @"
            SELECT s.Id, s.Nome, s.ValorTotal,
                   p.Id, p.SimulacaoId, p.NumeroParcela, p.ValorParcela
            FROM Simulacao s
            LEFT JOIN Parcelas p ON s.Id = p.SimulacaoId
            ORDER BY s.Id
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        ";

        var simulacoes = new Dictionary<int, Simulacao>();

        await _db.QueryAsync<Simulacao, Parcela, Simulacao>(
            sql,
            (simulacao, parcela) =>
            {
                if (!simulacoes.TryGetValue(simulacao.Id, out var existingSimulacao))
                {
                    existingSimulacao = simulacao;
                    existingSimulacao.Parcelas = new List<Parcela>();
                    simulacoes.Add(existingSimulacao.Id, existingSimulacao);
                }

                if (parcela != null)
                    existingSimulacao.Parcelas.Add(parcela);

                return existingSimulacao;
            },
            new
            {
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            },
            splitOn: "Id"
        );

        return simulacoes.Values.ToList();
    }

}
