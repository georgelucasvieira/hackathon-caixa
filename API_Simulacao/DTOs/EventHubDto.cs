namespace API_Simulacao.DTOs;

public class EventHubDTO
{
    public Guid Id { get; set; }
    public string? Aplicacao { get; set; }
    public DateTime DataHora { get; set; }
    public string? Mensagem { get; set; }

    public DetalheEventoDTO? Dados { get; set; }

    public EventHubDTO()
    {
        Id = Guid.NewGuid();
        DataHora = DateTime.Now;
    }

}
