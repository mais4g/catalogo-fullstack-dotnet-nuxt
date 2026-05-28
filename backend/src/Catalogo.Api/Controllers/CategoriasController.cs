using Catalogo.Api.Data;
using Catalogo.Api.DTOs;
using Catalogo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Api.Controllers;

[ApiController]
[Route("api/categorias")]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoriasController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaResponse>>> Listar()
    {
        var categorias = await _db.Categorias
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .Select(c => new CategoriaResponse(c.Id, c.Nome, c.Descricao))
            .ToListAsync();

        return Ok(categorias);
    }

    [HttpPost]
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

    [HttpPut("{id:int}")]
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

    [HttpDelete("{id:int}")]
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
