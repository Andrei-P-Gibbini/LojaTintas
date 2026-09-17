namespace LojaTintas.Application.DTOs.Categorias;

/// <summary>Representação de uma categoria retornada pela API.</summary>
public class CategoriaResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}
