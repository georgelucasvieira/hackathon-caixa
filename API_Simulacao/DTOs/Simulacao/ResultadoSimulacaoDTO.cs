using API_Simulacao.Enums;

namespace API_Simulacao.DTOs.Simulacao;

public class ResultadoSimulacaoDTO
{
    public TipoSimulacao tipo { get; set; }
    public IEnumerable<ParcelaSimulacaoDTO>? parcelas { get; set; }
}
