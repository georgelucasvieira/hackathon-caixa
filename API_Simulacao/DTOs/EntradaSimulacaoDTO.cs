namespace API_Simulacao.DTOs;

//public class EntradaSimulacaoDTO
//{
//    public decimal ValorDesejado { get; set; }
//    public int Prazo { get; set; }
//}

public record EntradaSimulacaoDTO(decimal ValorDesejado, int Prazo);