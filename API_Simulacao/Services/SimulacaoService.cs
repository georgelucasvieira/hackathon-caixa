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
        Produto? produto = await _produtoRepository.GetByValorEPrazoAsync(
            entradaSimulacaoDto.valorDesejado, entradaSimulacaoDto.prazo);
        if (produto is null)
            return new RetornoSimulacaoDTO();

        decimal valorDesejado = entradaSimulacaoDto.valorDesejado;
        int prazo = entradaSimulacaoDto.prazo;
        decimal taxaJuros = produto.PcTaxaJuros;

        var response = new RetornoSimulacaoDTO();
        if (produto is null)
            return response;

        var resultadosSimulacoes = new List<ResultadoSimulacaoDTO>
        {
            new ResultadoSimulacaoDTO
            {
                tipo = TipoSimulacao.SAC,
                parcelas = CalcularParcelasTabelaSAC(valorDesejado, prazo, taxaJuros)


            },
            new ResultadoSimulacaoDTO
            {
                tipo = TipoSimulacao.PRICE,
                parcelas = CalcularParcelasTabelaPrice(valorDesejado, prazo, taxaJuros)
            }
        };

        // TODO: persistir simulacao e retornar idSimulacao

        response.resultadoSimulacao = resultadosSimulacoes;
        response.codigoProduto = produto.CoProduto;
        response.taxaJuros = produto.PcTaxaJuros;
        response.idSimulacao = 1234;

        return response;
    }

    public List<ParcelaSimulacaoDTO> CalcularParcelasTabelaPrice(decimal valorDesejado, int prazo, decimal taxaJuros)
    {
        var retorno = new List<ParcelaSimulacaoDTO>();

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
                valorAmortizacao = Math.Round(valorAmortizacao, 2),
                valorJuros = Math.Round(valorJuros, 2),
                valorPrestacao = Math.Round(valorPrestacao, 2)
            };

            retorno.Add(dto);
        }

        return retorno;

    }

    public List<ParcelaSimulacaoDTO> CalcularParcelasTabelaSAC(decimal valorDesejado, int prazo, decimal taxaJuros)
    {
        var retorno = new List<ParcelaSimulacaoDTO>();

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
                valorAmortizacao = Math.Round(valorAmortizacao,2),
                valorJuros = Math.Round(valorJuros,2),
                valorPrestacao = Math.Round(valorPrestacao,2)
            });
        }

        return retorno;
    }


}
