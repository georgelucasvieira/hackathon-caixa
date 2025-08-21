namespace API_Simulacao.DTOs.Simulacao;

public class ParcelaSimulacaoDTO
{
    public int numero { get; set; }
    public decimal valorAmortizacao { get; set; }
    public decimal valorJuros { get; set; }
    public decimal valorPrestacao { get; set; }


    public override string ToString()
    {      
        return $"Parcela {numero}: Amortização = {valorAmortizacao:C}, Juros = {valorJuros:C}, Prestação = {valorPrestacao:C}";
    }
}
