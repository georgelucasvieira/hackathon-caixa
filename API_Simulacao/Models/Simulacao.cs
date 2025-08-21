namespace API_Simulacao.Models
{
    public class Simulacao
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public DateTime DataCriacao { get; set; }
        public ICollection<Parcela> Parcelas { get; set; }
    }
}
