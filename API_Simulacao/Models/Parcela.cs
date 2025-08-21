namespace API_Simulacao.Models
{
    public class Parcela
    {
        public int ParcelaId { get; set; }
        public int Numero { get; set; }
        public decimal ValorAmortizacao { get; set; }
        public decimal ValorJuros { get; set; }
        public decimal ValorPrestacao { get; set; }

    }
}
