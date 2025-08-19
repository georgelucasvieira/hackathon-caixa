using System.Text.Json.Serialization;

namespace API_Simulacao.DTOs;

public class EventHubDto
{
    public Guid Id { get; set; }
    public string? Aplicacao { get; set; }
    public DateTime DataHora { get; set; }
    public string? Mensagem { get; set; }

    public DetalheEventoDto? Dados { get; set; }

    public EventHubDto()
    {
        Id = Guid.NewGuid();
        DataHora = DateTime.Now;
    }

}
