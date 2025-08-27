using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API_Simulacao.DTOs.Simulacao;

public class EntradaSimulacaoDTO
{
    [Required(ErrorMessage = "O valor desejado é obrigatório.")]
    [Range(1, double.MaxValue, ErrorMessage = "O valor desejado deve ser maior que zero.")]
    [DefaultValue(10000)]
    public decimal valorDesejado { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "O prazo deve ser maior que zero.")]
    [DefaultValue(24)]
    public int prazo { get; set; }
}
