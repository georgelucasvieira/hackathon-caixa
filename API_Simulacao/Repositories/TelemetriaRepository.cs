using System.Data;
using Dapper;
using Npgsql;

namespace API_Simulacao.Models;

public class TelemetriaRepository
{
    public IDbConnection _db { get; set; }
    public TelemetriaRepository(IConfiguration configuration)
    {
        _db = new NpgsqlConnection(configuration.GetConnectionString("DbSimulacao"));
    }

    public async Task<List<MetricasDTO>> ObterMetricasAsync(DateTime dataReferencia)
    {
        var sql = @"
            SELECT 
                NOME_API as nomeApi, 
                ROUND(AVG(TEMPO_MS)::numeric, 2) AS tempoMedio,
                MIN(TEMPO_MS) AS tempoMinimo,
                MAX(TEMPO_MS) AS tempoMaximo,
                COUNT(*) AS qtdeRequisicoes,
                ROUND(
                    (SUM(CASE WHEN STATUS_CODE >= 200 AND STATUS_CODE < 300 THEN 1 ELSE 0 END)::numeric / COUNT(*)),
                    2
                ) AS percentualSucesso
            FROM METRICAS_TELEMETRIA
            WHERE DATE(DATA) = @DataReferencia
            GROUP BY NOME_API;
        ";

        var metricas = (await _db.QueryAsync<MetricasDTO>(sql,
            new { DataReferencia = dataReferencia }
        )).ToList();

        return metricas;
    }

    public async Task GravarMetricaAsync(MetricasTelemetria telemetria)
    {
        var sql = @"
            INSERT INTO METRICAS_TELEMETRIA (NOME_API, TEMPO_MS, STATUS_CODE, DATA)
            VALUES (@NomeApi, @TempoMs, @StatusCode, @Data);
        ";
        await _db.ExecuteScalarAsync<int>(sql, telemetria);
    }
    
}