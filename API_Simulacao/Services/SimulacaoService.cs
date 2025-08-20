using API_Simulacao.DTOs.Simulacao;
using API_Simulacao.Enums;
using API_Simulacao.Models;
using API_Simulacao.Repositories;

namespace API_Simulacao.Services;

public class SimulacaoService
{
    private readonly ProdutoRepository _produtoRepository;
    private readonly SimulacaoRepository _simulacaoRepository;

    public SimulacaoService(ProdutoRepository produtoRepository, SimulacaoRepository simulacaoRepository)
    {
        _produtoRepository = produtoRepository;
        _simulacaoRepository = simulacaoRepository;
    }

    public async Task<RetornoSimulacaoDTO> RealizarSimulacao(EntradaSimulacaoDTO entradaSimulacaoDto)
    {
        Produto produto = await _produtoRepository.GetByValorEPrazoAsync(
            entradaSimulacaoDto.valorDesejado, entradaSimulacaoDto.prazo);

        var response = new RetornoSimulacaoDTO();
        if (produto is null)
            return response;

        var resultadosSimulacoes = new List<ResultadoSimulacaoDTO>();
        resultadosSimulacoes.Add(new ResultadoSimulacaoDTO
        {
            tipo = TipoSimulacao.SAC
        });
        resultadosSimulacoes.Add(new ResultadoSimulacaoDTO
        {
            tipo = TipoSimulacao.PRICE
        });

        //TODO: persistir simulacao e retornar idSimulacao

        response.resultadoSimulacao = resultadosSimulacoes;
        response.codigoProduto = produto.CoProduto;
        response.taxaJuros = produto.PcTaxaJuros;
        response.idSimulacao = 1234;

        return response;
    }

    public static async Task<List<Parcela>> CalcularTabelaPrice() { return null; }

    public static async Task<List<Parcela>> CalcularTabelaSAC() { return null; }


}
