namespace Catalogo.Api.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    public List<Produto> Produtos { get; set; } = new();
}
