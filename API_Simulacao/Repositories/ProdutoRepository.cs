using API_Simulacao.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;


namespace API_Simulacao.Repositories;

public class ProdutoRepository
{
    private readonly IDbConnection _db;
    private readonly IConfiguration _configuration;

    public ProdutoRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _db = new SqlConnection(_configuration.GetConnectionString("DbProduto"));
    }

    //public async Task<IEnumerable<Produto>> GetAllAsync()
    //{
    //    var sql = "SELECT * FROM dbo.PRODUTO";
    //    return await _db.QueryAsync<Produto>(sql);
    //}

    //public async Task<Produto?> GetByIdAsync(int id)
    //{
    //    var sql = "SELECT * FROM dbo.PRODUTO WHERE CO_PRODUTO = @Id";
    //    return await _db.QuerySingleOrDefaultAsync<Produto>(sql, new { Id = id });
    //}
    public async Task<Produto> GetByValorEPrazoAsync(decimal valorDesejado, int prazo)
    {
        var sql = @"
            SELECT * FROM PRODUTO
            WHERE VR_MAXIMO >= @ValorDesejado AND VR_MINIMO <= @ValorDesejado
            AND NU_MINIMO_MESES <= @Prazo AND NU_MAXIMO_MESES >= @Prazo
        ";
        return await _db.QueryFirstAsync<Produto>(sql, new { ValorDesejado = valorDesejado, Prazo = prazo });
    }
}
