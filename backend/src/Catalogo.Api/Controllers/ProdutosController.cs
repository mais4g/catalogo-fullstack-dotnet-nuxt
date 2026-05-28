using Catalogo.Api.Data;
using Catalogo.Api.DTOs;
using Catalogo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Api.Controllers;

/// <summary>
/// Endpoints para gerenciamento de produtos do catálogo.
/// </summary>
[ApiController]
[Route("api/produtos")]
[Produces("application/json")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProdutosController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Lista todos os produtos com a categoria vinculada como objeto aninhado.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProdutoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProdutoResponse>>> Listar()
    {
        var produtos = await _db.Produtos
            .AsNoTracking()
            .Include(p => p.Categoria)
            .OrderBy(p => p.Nome)
            .Select(p => new ProdutoResponse(
                p.Id,
                p.Nome,
                p.Descricao,
                p.Preco,
                p.CategoriaId,
                new CategoriaResponse(p.Categoria.Id, p.Categoria.Nome, p.Categoria.Descricao)
            ))
            .ToListAsync();

        return Ok(produtos);
    }

    /// <summary>
    /// Cria um novo produto vinculado a uma categoria existente.
    /// </summary>
    /// <remarks>
    /// O campo Nome é obrigatório e deve ter no mínimo 5 caracteres.
    /// O CategoriaId deve referenciar uma categoria existente.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProdutoResponse>> Criar(ProdutoRequest request)
    {
        var categoria = await _db.Categorias.FindAsync(request.CategoriaId);
        if (categoria is null)
        {
            return BadRequest(new
            {
                mensagem = $"Categoria com id {request.CategoriaId} não existe."
            });
        }

        var produto = new Produto
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            Preco = request.Preco,
            CategoriaId = request.CategoriaId
        };

        _db.Produtos.Add(produto);
        await _db.SaveChangesAsync();

        var response = new ProdutoResponse(
            produto.Id,
            produto.Nome,
            produto.Descricao,
            produto.Preco,
            produto.CategoriaId,
            new CategoriaResponse(categoria.Id, categoria.Nome, categoria.Descricao)
        );

        return CreatedAtAction(nameof(Listar), new { id = produto.Id }, response);
    }

    /// <summary>
    /// Atualiza um produto existente pelo id.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoResponse>> Atualizar(int id, ProdutoRequest request)
    {
        var produto = await _db.Produtos.FindAsync(id);
        if (produto is null)
        {
            return NotFound();
        }

        var categoria = await _db.Categorias.FindAsync(request.CategoriaId);
        if (categoria is null)
        {
            return BadRequest(new
            {
                mensagem = $"Categoria com id {request.CategoriaId} não existe."
            });
        }

        produto.Nome = request.Nome;
        produto.Descricao = request.Descricao;
        produto.Preco = request.Preco;
        produto.CategoriaId = request.CategoriaId;
        await _db.SaveChangesAsync();

        return Ok(new ProdutoResponse(
            produto.Id,
            produto.Nome,
            produto.Descricao,
            produto.Preco,
            produto.CategoriaId,
            new CategoriaResponse(categoria.Id, categoria.Nome, categoria.Descricao)
        ));
    }

    /// <summary>
    /// Remove um produto pelo id.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id)
    {
        var produto = await _db.Produtos.FindAsync(id);
        if (produto is null)
        {
            return NotFound();
        }

        _db.Produtos.Remove(produto);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
