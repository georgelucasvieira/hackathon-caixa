using API_Simulacao.Enums;
using System.Text.Json.Serialization;

namespace API_Simulacao.DTOs.Simulacao;

public class ResultadoSimulacaoDTO
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TipoSimulacao tipo { get; set; }
    public IEnumerable<ParcelaSimulacaoDTO>? parcelas { get; set; }
}
