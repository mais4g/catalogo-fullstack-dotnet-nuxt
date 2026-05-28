using Catalogo.Api.Data;
using Catalogo.Api.DTOs;
using Catalogo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Api.Controllers;

/// <summary>
/// Endpoints para gerenciamento de categorias do catálogo.
/// </summary>
[ApiController]
[Route("api/categorias")]
[Produces("application/json")]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoriasController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Lista todas as categorias cadastradas, ordenadas por nome.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoriaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoriaResponse>>> Listar()
    {
        var categorias = await _db.Categorias
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .Select(c => new CategoriaResponse(c.Id, c.Nome, c.Descricao))
            .ToListAsync();

        return Ok(categorias);
    }

    /// <summary>
    /// Cria uma nova categoria.
    /// </summary>
    /// <remarks>O campo Nome é obrigatório e deve ter no mínimo 5 caracteres.</remarks>
    [HttpPost]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoriaResponse>> Criar(CategoriaRequest request)
    {
        var categoria = new Categoria
        {
            Nome = request.Nome,
            Descricao = request.Descricao
        };

        _db.Categorias.Add(categoria);
        await _db.SaveChangesAsync();

        var response = new CategoriaResponse(categoria.Id, categoria.Nome, categoria.Descricao);
        return CreatedAtAction(nameof(Listar), new { id = categoria.Id }, response);
    }

    /// <summary>
    /// Atualiza uma categoria existente pelo id.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaResponse>> Atualizar(int id, CategoriaRequest request)
    {
        var categoria = await _db.Categorias.FindAsync(id);
        if (categoria is null)
        {
            return NotFound();
        }

        categoria.Nome = request.Nome;
        categoria.Descricao = request.Descricao;
        await _db.SaveChangesAsync();

        return Ok(new CategoriaResponse(categoria.Id, categoria.Nome, categoria.Descricao));
    }

    /// <summary>
    /// Remove uma categoria pelo id.
    /// </summary>
    /// <remarks>
    /// Retorna 409 Conflict se a categoria possuir produtos vinculados,
    /// preservando a integridade referencial do catálogo.
    /// </remarks>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Remover(int id)
    {
        var categoria = await _db.Categorias.FindAsync(id);
        if (categoria is null)
        {
            return NotFound();
        }

        var possuiProdutos = await _db.Produtos.AnyAsync(p => p.CategoriaId == id);
        if (possuiProdutos)
        {
            return Conflict(new
            {
                mensagem = "Não é possível excluir uma categoria que possua produtos vinculados."
            });
        }

        _db.Categorias.Remove(categoria);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
