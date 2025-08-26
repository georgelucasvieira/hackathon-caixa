namespace API_Simulacao.DTOs.Simulacao;

public class EntradaSimulacaoDTO
{
   public decimal valorDesejado { get; set; }
   public int prazo { get; set; }
}

// public record EntradaSimulacaoDTO(decimal valorDesejado, int prazo);