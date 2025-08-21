namespace API_Simulacao.DTOs;

public class RetornoPaginadoDTO<T>
{
    public int pagina { get; set; }
    public int qtdRegistros { get; set; }
    public int qtdRegistrosPagina { get; set; }
    public List<T> registros { get; set; }
}
