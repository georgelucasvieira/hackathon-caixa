namespace API_Simulacao.Models
{
    public class SolicitacaoSimulacao
    {
        public int Id { get; set; }
        public DateTime DataCriacao { get; set; }
        public int Prazo { get; set; }
        public decimal ValorDesejado { get; set; }
        public ICollection<Simulacao>? Simulacoes { get; set; }
        public int CoProduto { get; set; }
        public string? NomeProduto { get; set; }
    }
}
