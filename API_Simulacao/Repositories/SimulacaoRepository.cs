using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace API_Simulacao.Repositories;

public class SimulacaoRepository
{
    private readonly IDbConnection  _db;
    private readonly IConfiguration _configuration;
    public SimulacaoRepository(IConfiguration configuration) 
    { 
        _configuration = configuration;
        _db = new SqlConnection(_configuration.GetConnectionString("ProdutoDb"))
    }
}
