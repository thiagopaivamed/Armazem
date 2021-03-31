using Armazem.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Armazem.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly IConfiguration _configuracao;
        private readonly NpgsqlConnection _conexao;

        public ProdutosController(IConfiguration configuracao)
        {
            _configuracao = configuracao;
            _conexao = new NpgsqlConnection(_configuracao.GetConnectionString("ConexaoBD"));
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                string selecaoQuery = "SELECT p.produtoid, p.nome, p.preco, c.categoriaid, c.nome"
                            + " FROM produtos AS p"
                            + " INNER JOIN categorias AS c"
                            + " ON p.categoriaid = c.categoriaid";

                await _conexao.OpenAsync();
                IEnumerable<Produto> listaProdutos = await _conexao.QueryAsync<Produto, Categoria, Produto>(selecaoQuery, (produto, categoria) =>
                {
                    produto.Categoria = categoria;
                    return produto;
                },
                splitOn: "CategoriaId");

                return View(listaProdutos);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                await _conexao.CloseAsync();
            }            
        }

        [HttpGet]
        public async Task<IActionResult> NovoProduto()
        {
            try
            {
                string selecaoQuery = "SELECT * FROM categorias";
                await _conexao.OpenAsync();
                IEnumerable<Categoria> listaCategorias = await _conexao.QueryAsync<Categoria>(selecaoQuery);
                ViewData["opcoesCategoria"] = new SelectList(listaCategorias.ToList(), "CategoriaId", "Nome");
                return View();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                await _conexao.CloseAsync();
            }
           
        }
        
        [HttpPost]
        public async Task<IActionResult> NovoProduto(Produto produto)
        {
            try
            {
                await _conexao.OpenAsync();
                if (ModelState.IsValid)
                {
                    string insercaoQuery = "INSERT INTO produtos(nome, preco, categoriaid) VALUES (@nome, @preco, @categoriaId)";                   
                    await _conexao.ExecuteAsync(insercaoQuery, produto);
                    TempData["ProdutoCriado"] = $"Produto {produto.Nome} criado com sucesso";
                    return RedirectToAction(nameof(Index));
                }

                string selecaoQuery = "SELECT * FROM categorias";                
                IEnumerable<Categoria> listaCategorias = await _conexao.QueryAsync<Categoria>(selecaoQuery);
                ViewData["opcoesCategoria"] = new SelectList(listaCategorias.ToList(), "CategoriaId", "Nome", produto.CategoriaId);
                return View(produto);

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                await _conexao.CloseAsync();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AtualizarProduto(int produtoId)
        {
            try
            {
                string selecaoProdutoQuery = $"SELECT * FROM produtos WHERE produtoid = {produtoId}";
                string selecaoCategoriasQuery = "SELECT * FROM categorias";

                await _conexao.OpenAsync();
                Produto produto = await _conexao.QueryFirstAsync<Produto>(selecaoProdutoQuery, new { produtoid = produtoId });
                IEnumerable<Categoria> listaCategoria = await _conexao.QueryAsync<Categoria>(selecaoCategoriasQuery);

                ViewData["opcoesCategoria"] = new SelectList(listaCategoria.ToList(), "CategoriaId", "Nome", produto.CategoriaId);

                return View(produto);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                await _conexao.CloseAsync();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarProduto(Produto produto)
        {
            try
            {
                await _conexao.OpenAsync();
                if (ModelState.IsValid)
                {
                    string atualizacaoQuery = "UPDATE produtos SET nome = @nome, preco = @preco, categoriaid = @categoriaid WHERE produtoid = @produtoid";
                    await _conexao.ExecuteAsync(atualizacaoQuery, produto);
                    TempData["ProdutoAtualizado"] = $"Produto {produto.Nome} atualizado com sucesso";
                    return RedirectToAction(nameof(Index));
                }

                string selecaoCategoriasQuery = "SELECT * FROM categorias";
                IEnumerable<Categoria> listaCategoria = await _conexao.QueryAsync<Categoria>(selecaoCategoriasQuery);

                ViewData["opcoesCategoria"] = new SelectList(listaCategoria.ToList(), "CategoriaId", "Nome", produto.CategoriaId);

                return View(produto);

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                await _conexao.CloseAsync();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExcluirProduto(int produtoId)
        {
            try
            {
                string exclusaoQuery = "DELETE FROM produtos WHERE produtoid = @produtoid";
                await _conexao.OpenAsync();
                await _conexao.ExecuteAsync(exclusaoQuery, new { produtoid = produtoId });
                TempData["ProdutoExcluido"] = $"Produto excluído com sucesso";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                await _conexao.CloseAsync();
            }
        }
    }
}
