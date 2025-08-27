namespace API_Simulacao.DTOs
{
    public class RetornoListaSimulacaoDTO
    {
        public int idSimulacao { get; set; }
        public decimal valorDesejado { get; set; }
        public int prazo { get; set; }
        public decimal valorTotalParcelas { get; set; }
    }
}
