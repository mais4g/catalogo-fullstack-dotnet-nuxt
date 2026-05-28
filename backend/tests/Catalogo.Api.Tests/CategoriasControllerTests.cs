using System.Net;
using System.Net.Http.Json;
using Catalogo.Api.Data;
using Catalogo.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Catalogo.Api.Tests;

public class CategoriasControllerTests : IDisposable
{
    private readonly CatalogoApiFactory _factory;
    private readonly HttpClient _client;

    public CategoriasControllerTests()
    {
        _factory = new CatalogoApiFactory();
        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }

    [Fact]
    public async Task DeleteCategoria_ComProdutosVinculados_Retorna409ComMensagemLiteral()
    {
        int categoriaId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var categoria = new Categoria { Nome = "Eletrônicos" };
            db.Categorias.Add(categoria);
            await db.SaveChangesAsync();

            db.Produtos.Add(new Produto
            {
                Nome = "Notebook Dell",
                Preco = 4599.90m,
                CategoriaId = categoria.Id
            });
            await db.SaveChangesAsync();

            categoriaId = categoria.Id;
        }

        var response = await _client.DeleteAsync($"/api/categorias/{categoriaId}");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains(
            "Não é possível excluir uma categoria que possua produtos vinculados.",
            body);
    }

    [Fact]
    public async Task CriarCategoria_NomeMenorQue5_RetornaBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/categorias", new { nome = "abc" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CriarCategoria_NomeNulo_RetornaBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/categorias",
            new { descricao = "Sem nome" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
