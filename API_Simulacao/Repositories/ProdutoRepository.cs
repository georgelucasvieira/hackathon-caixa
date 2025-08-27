using API_Simulacao.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using System.Data;


namespace API_Simulacao.Repositories;

public class ProdutoRepository
{
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private const string CacheKeyAll = "Produtos:All";

    public ProdutoRepository(IConfiguration configuration, IMemoryCache cache)
    {
        _configuration = configuration;
        _cache = cache;
    }
    public async Task<Produto?> GetByValorEPrazoAsync(decimal valorDesejado, int prazo)
    {
        var produtos = await GetProdutosAsync();

        return produtos.FirstOrDefault(p =>
                         p.VrMinimo <= valorDesejado && p.VrMaximo >= valorDesejado &&
                         p.NuMinimoMeses <= prazo && p.NuMaximoMeses >= prazo);
    }
    public async Task<IEnumerable<Produto>?> GetProdutosAsync(Func<Produto, bool>? filtro = null)
    {
        var produtos = await _cache.GetOrCreateAsync(CacheKeyAll, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));
            entry.SetAbsoluteExpiration(TimeSpan.FromHours(6));
            var sql = @"
                SELECT CO_PRODUTO as CoProduto,
                    NO_PRODUTO as NomeProduto, 
                    VR_MINIMO as VrMinimo, 
                    VR_MAXIMO as VrMaximo, 
                    NU_MINIMO_MESES as NuMinimoMeses, 
                    NU_MAXIMO_MESES as NuMaximoMeses, 
                    PC_TAXA_JUROS as PcTaxaJuros
                FROM PRODUTO
            ";
            Console.WriteLine("Carregando produtos do banco de dados... " + sql);
            IDbConnection _db = new SqlConnection(_configuration.GetConnectionString("DbProduto"));
            return await _db.QueryAsync<Produto>(sql);
        });

        if (produtos is null)
            return Enumerable.Empty<Produto>();

        return filtro == null ? produtos : produtos.Where(filtro).ToList();
    }
}
