using System.Text.Json.Serialization;
using API_Simulacao.Util;

namespace API_Simulacao.Models;

public class RetornoTelemetriaDTO
{
    [JsonConverter(typeof(JsonDateConverter))]
    public DateTime dataReferencia { get; set; }
    public List<MetricasDTO> listaEndpoints { get; set; } = new List<MetricasDTO>();

}

public class MetricasDTO
{
    public string nomeApi { get; set; } = null!;
    public int qtdeRequisicoes { get; set; }
    public int tempoMedio { get; set; }
    public int tempoMinimo { get; set; }
    public int tempoMaximo { get; set; }
    public double percentualSucesso { get; set; }
}