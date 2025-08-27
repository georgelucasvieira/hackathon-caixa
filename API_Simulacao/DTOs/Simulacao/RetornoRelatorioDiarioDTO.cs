using System.Text.Json.Serialization;
using API_Simulacao.Util;

namespace API_Simulacao.DTOs.Simulacao;

public class RetornoRelatorioDiarioDTO
{
    [JsonConverter(typeof(JsonDateConverter))]
    public DateTime dataReferencia { get; set; }
    public List<SimulacaoRelatorioDiarioDTO> simulacoes { get; set; } = new List<SimulacaoRelatorioDiarioDTO>();
}

public class SimulacaoRelatorioDiarioDTO
{
    public int codigoProduto { get; set; }
    public string descricaoProduto { get; set; } = string.Empty;
    public decimal taxaMediaJuro { get; set; }
    public decimal valorMedioPrestacao { get; set; }
    public decimal valorTotalDesejado { get; set; }
    public decimal valorTotalCredito { get; set; }
}