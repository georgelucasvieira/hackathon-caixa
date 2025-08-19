namespace API_Simulacao.DTOs.Simulacao;

//public class EntradaSimulacaoDTO
//{
//    public decimal ValorDesejado { get; set; }
//    public int Prazo { get; set; }
//}

public record EntradaSimulacaoDTO(decimal valorDesejado, int prazo);