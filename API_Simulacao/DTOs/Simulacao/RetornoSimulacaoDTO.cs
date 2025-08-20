namespace API_Simulacao.DTOs.Simulacao;

public class RetornoSimulacaoDTO
{
    public int idSimulacao { get; set; }
    public int codigoProduto { get; set; }
    public decimal taxaJuros { get; set; }
    public List<ResultadoSimulacaoDTO>? resultadoSimulacao { get; set; }
}
