using System.ComponentModel.DataAnnotations;

namespace Catalogo.Api.DTOs;

public record ProdutoRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MinLength(5, ErrorMessage = "O nome deve ter no mínimo 5 caracteres.")]
    [MaxLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres.")]
    string Nome,

    [MaxLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres.")]
    string? Descricao,

    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
    decimal Preco,

    [Range(1, int.MaxValue, ErrorMessage = "A categoria é obrigatória.")]
    int CategoriaId
);
