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
            tipo = TipoSimulacao.SAC,
            parcelas = CalcularParcelasTabelaSAC()

        });
        resultadosSimulacoes.Add(new ResultadoSimulacaoDTO
        {
            tipo = TipoSimulacao.PRICE,
            parcelas = CalcularParcelasTabelaPrice()
        });

        // TODO: persistir simulacao e retornar idSimulacao

        response.resultadoSimulacao = resultadosSimulacoes;
        response.codigoProduto = produto.CoProduto;
        response.taxaJuros = produto.PcTaxaJuros;
        response.idSimulacao = 1234;

        return response;
    }

    public List<ParcelaSimulacaoDTO> CalcularParcelasTabelaPrice()
    {
        var retorno = new List<ParcelaSimulacaoDTO>();

        var valorDesejado = (decimal)30000.00;
        var taxaJuros = (decimal)0.020;
        var prazo = 24;

        var fator = (decimal)Math.Pow((double)(1 + taxaJuros), -prazo);
        var valorPrestacao = valorDesejado * taxaJuros / (1 - fator);

        var saldoDevedor = valorDesejado;

        for (int i = 1; i <= prazo; i++)
        {
            var valorJuros = saldoDevedor * taxaJuros;
            var valorAmortizacao = valorPrestacao - valorJuros;
            saldoDevedor -= valorAmortizacao;

            var dto = new ParcelaSimulacaoDTO
            {
                numero = i,
                valorAmortizacao = Math.Round(valorAmortizacao),
                valorJuros = Math.Round(valorJuros, 2),
                valorPrestacao = Math.Round(valorPrestacao, 2)
            };

            retorno.Add(dto);
        }

        return retorno;

    }

    public List<ParcelaSimulacaoDTO> CalcularParcelasTabelaSAC()
    {
        var retorno = new List<ParcelaSimulacaoDTO>();

        var valorDesejado = (decimal)30000.00;
        var taxaJuros = (decimal)0.020;
        var prazo = 24;

        var valorAmortizacao = valorDesejado / prazo;
        var auxSaldoDevedor = valorDesejado;

        for (int i = 1; i <= prazo; i++)
        {
            var valorJuros = auxSaldoDevedor * taxaJuros;
            var valorPrestacao = valorAmortizacao + valorJuros;
            auxSaldoDevedor -= valorAmortizacao;

            retorno.Add(new ParcelaSimulacaoDTO
            {
                numero = i,
                valorAmortizacao = valorAmortizacao,
                valorJuros = valorJuros,
                valorPrestacao = valorPrestacao
            });
        }

        return retorno;
    }


}
