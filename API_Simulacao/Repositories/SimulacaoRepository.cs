using API_Simulacao.Models;
using Dapper;
using Npgsql;
using System.Data;

namespace API_Simulacao.Repositories;

public class SimulacaoRepository
{
    private readonly IDbConnection  _db;
    private readonly IConfiguration _configuration;
    public SimulacaoRepository(IConfiguration configuration) 
    { 
        _configuration = configuration;
        _db = new NpgsqlConnection(_configuration.GetConnectionString("DbSimulacao"));
    }


    public async Task<List<Simulacao>> GetAllPaginatedAsync(int pageNumber, int pageSize)
    {
        var sql = @"
        WITH PaginatedSimulacoes AS (
            SELECT *
            FROM SIMULACAO
            ORDER BY ID
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
        )
        SELECT 
            s.ID as Id, s.TIPO as Tipo, s.DATA_CRIACAO AS DataCriacao,
            p.ID as ParcelaId, p.SIMULACAO_ID as SimulacaoId, p.NUMERO as Numero,
            p.VALOR_AMORTIZACAO AS ValorAmortizacao, p.VALOR_JUROS AS ValorJuros, p.VALOR_PRESTACAO AS ValorPrestacao
        FROM PaginatedSimulacoes s
        LEFT JOIN PARCELAS p ON s.ID = p.SIMULACAO_ID
        ORDER BY s.ID, p.NUMERO;
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
            splitOn: "ParcelaId"
        );
        
        return simulacoes.Values.ToList();
    }
}
