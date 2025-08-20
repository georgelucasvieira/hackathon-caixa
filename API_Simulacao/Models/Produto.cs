namespace API_Simulacao.Models;
public class Produto
{
    public int CoProduto { get; set; }
    public string? NomeProduto { get; set; }
    public decimal PcTaxaJuros { get; set; }
    public short NuMinimoMeses { get; set; }
    public short? NuMaximoMeses { get; set; }
    public decimal VrMinimo { get; set; }
    public decimal? VrMaximo { get; set; }
}

