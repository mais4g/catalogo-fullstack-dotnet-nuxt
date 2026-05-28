using System.Net.Http.Json;
using Catalogo.Api.Data;
using Catalogo.Api.DTOs;
using Catalogo.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Catalogo.Api.Tests;

public class ProdutosControllerTests : IDisposable
{
    private readonly CatalogoApiFactory _factory;
    private readonly HttpClient _client;

    public ProdutosControllerTests()
    {
        _factory = new CatalogoApiFactory();
        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }

    [Fact]
    public async Task ListarProdutos_IncluiObjetoCategoriaAninhado()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var categoria = new Categoria { Nome = "Eletrônicos", Descricao = "Aparelhos" };
            db.Categorias.Add(categoria);
            await db.SaveChangesAsync();

            db.Produtos.Add(new Produto
            {
                Nome = "Notebook Dell",
                Preco = 4599.90m,
                CategoriaId = categoria.Id
            });
            await db.SaveChangesAsync();
        }

        var produtos = await _client.GetFromJsonAsync<List<ProdutoResponse>>("/api/produtos");

        Assert.NotNull(produtos);
        Assert.Single(produtos);
        Assert.NotNull(produtos[0].Categoria);
        Assert.Equal("Eletrônicos", produtos[0].Categoria.Nome);
        Assert.Equal("Aparelhos", produtos[0].Categoria.Descricao);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
