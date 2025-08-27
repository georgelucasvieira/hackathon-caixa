using API_Simulacao.DTOs;
using API_Simulacao.DTOs.Simulacao;
using API_Simulacao.Enums;
using Dapper;
using Npgsql;
using System.Data;

namespace API_Simulacao.Repositories;

public class SimulacaoRepository
{
    private readonly IDbConnection _db;
    private readonly IConfiguration _configuration;
    public SimulacaoRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _db = new NpgsqlConnection(_configuration.GetConnectionString("DbSimulacao"));
    }

    public async Task<RetornoPaginadoDTO<RetornoListaSimulacaoDTO>> GetAllByTipoPaginatedAsync(int pageNumber, int pageSize, TipoSimulacao tipo)
    {
        var countSql = "SELECT COUNT(*) FROM SIMULACAO WHERE TIPO = @Tipo";
        var totalRegistros = await _db.ExecuteScalarAsync<int>(countSql, new { Tipo = tipo.ToString() });

        var dataSql = @"
            SELECT 
                s.ID as id,
                ss.ID AS idSimulacao,
                s.TIPO AS tipoSimulacao,
                ss.PRAZO AS prazo,
                ss.VALOR_DESEJADO AS valorDesejado,
                SUM(p.VALOR_PRESTACAO) AS valorTotalParcelas
            FROM SIMULACAO s
            LEFT JOIN PARCELAS p ON s.ID = p.SIMULACAO_ID
            LEFT JOIN SOLICITACAO_SIMULACAO ss ON ss.ID = s.SOLICITACAO_ID
            WHERE s.TIPO = @Tipo
            GROUP BY s.ID, ss.ID, s.TIPO, ss.PRAZO
            ORDER BY s.ID
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        ";

        var simulacoes = (await _db.QueryAsync<RetornoListaSimulacaoDTO>(
        dataSql,
        new
        {
            Tipo = tipo.ToString(),
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        }
        )).ToList();

        return new RetornoPaginadoDTO<RetornoListaSimulacaoDTO>
        {
            pagina = pageNumber,
            qtdRegistros = totalRegistros,
            qtdRegistrosPagina = pageSize,
            registros = simulacoes
        };
    }

    public async Task<List<SimulacaoRelatorioDiarioDTO>> GetAllByDataProdutoTipoAsync(DateTime dataReferencia, int coProduto, TipoSimulacao tipo)
    {
        var sql = @"
            SELECT 
                ss.CO_PRODUTO AS codigoProduto,
                ss.NO_PRODUTO AS descricaoProduto,
                SUM(ss.VALOR_DESEJADO) AS valorTotalDesejado,
                AVG(p.VALOR_JUROS) AS taxaMediaJuro,
                AVG(p.VALOR_PRESTACAO) AS valorMedioPrestacao,
                SUM(p.VALOR_PRESTACAO) AS valorTotalCredito
            FROM SIMULACAO s
            LEFT JOIN PARCELAS p ON s.ID = p.SIMULACAO_ID
            LEFT JOIN SOLICITACAO_SIMULACAO ss ON ss.ID = s.SOLICITACAO_ID
            WHERE DATE(ss.DATA_CRIACAO) = @Data AND ss.CO_PRODUTO = @CoProduto AND s.TIPO = @Tipo
            GROUP BY s.ID, ss.ID, s.TIPO, ss.PRAZO
            ORDER BY s.ID;
        ";

        var simulacoes = (await _db.QueryAsync<SimulacaoRelatorioDiarioDTO>(
            sql, new { Data = dataReferencia.Date, CoProduto = coProduto, Tipo = tipo.ToString() })
        ).ToList();

        return simulacoes;
    }

    public async Task<int> InserirSimulacaoCompletaAsync(decimal valorDesejado, int prazo, int coProduto, string nomeProduto, List<ResultadoSimulacaoDTO> resultadosSimulacoes)
    {
        _db.Open();
        using var transaction = _db.BeginTransaction();

        try
        {
            var solicitacaoSql = @"
                INSERT INTO SOLICITACAO_SIMULACAO (PRAZO, VALOR_DESEJADO, DATA_CRIACAO, CO_PRODUTO, NO_PRODUTO)
                VALUES (@Prazo, @ValorDesejado, NOW(), @CoProduto, @NomeProduto)
                RETURNING ID;
            ";

            var solicitacaoId = await _db.ExecuteScalarAsync<int>(
                solicitacaoSql,
                new { Prazo = prazo, ValorDesejado = valorDesejado, CoProduto = coProduto, NomeProduto = nomeProduto },
                transaction
            );

            foreach (var resultado in resultadosSimulacoes)
            {
                var sql = "INSERT INTO SIMULACAO (SOLICITACAO_ID, TIPO, DATA_CRIACAO) VALUES (@SolicitacaoId, @Tipo, NOW()) RETURNING ID;";
                var simulacaoId = await _db.ExecuteScalarAsync<int>(sql, new { SolicitacaoId = solicitacaoId, Tipo = resultado.tipo.ToString() }, transaction);

                foreach (var parcela in resultado.parcelas!)
                {
                    var parcelaSql = @"
                        INSERT INTO PARCELAS (SIMULACAO_ID, NUMERO, VALOR_PRESTACAO, VALOR_AMORTIZACAO, VALOR_JUROS)
                        VALUES (@SimulacaoId, @NumeroParcela, @ValorPrestacao, @ValorAmortizacao, @ValorJuros);
                    ";
                    await _db.ExecuteAsync(parcelaSql, new
                    {
                        SimulacaoId = simulacaoId,
                        NumeroParcela = parcela.numero,
                        ValorPrestacao = parcela.valorPrestacao,
                        ValorAmortizacao = parcela.valorAmortizacao,
                        ValorJuros = parcela.valorJuros,
                    }, transaction);
                }
            }

            transaction.Commit();
            return solicitacaoId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

}
