namespace Catalogo.Api.DTOs;

public record ProdutoResponse(
    int Id,
    string Nome,
    string? Descricao,
    decimal Preco,
    int CategoriaId,
    CategoriaResponse Categoria
);
