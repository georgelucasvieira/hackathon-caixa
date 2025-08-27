namespace API_Simulacao.Models;

public class MetricasTelemetria
{
    public int Id { get; set; }
    public string NomeApi { get; set; } = null!;
    public int TempoMs { get; set; }
    public int StatusCode { get; set; }
    public DateTime Data { get; set; }
}