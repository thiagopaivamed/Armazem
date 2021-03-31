using Armazem.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Armazem.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IConfiguration _configuracao;
        private readonly NpgsqlConnection _conexao;

        public CategoriasController(IConfiguration configuracao)
        {
            _configuracao = configuracao;
            _conexao = new NpgsqlConnection(_configuracao.GetConnectionString("ConexaoBD"));
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                string selecaoQuery = "SELECT * FROM categorias";
                await _conexao.OpenAsync();
                IEnumerable<Categoria> listaCategorias = await _conexao.QueryAsync<Categoria>(selecaoQuery);

                return View(listaCategorias);
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
        public IActionResult NovaCategoria()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NovaCategoria(Categoria categoria)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string insercaoQuery = "INSERT INTO categorias(nome) VALUES(@nome)";
                    await _conexao.OpenAsync();
                    await _conexao.ExecuteAsync(insercaoQuery, categoria);
                    TempData["CategoriaCriada"] = $"Categoria {categoria.Nome} criada com sucesso";

                    return RedirectToAction(nameof(Index));
                }

                return View(categoria);
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
        public async Task<IActionResult> AtualizarCategoria(int categoriaId)
        {
            try
            {
                string selecaoQuery = $"SELECT * FROM categorias WHERE categoriaid = {categoriaId}";
                await _conexao.OpenAsync();
                Categoria categoria = await _conexao.QueryFirstAsync<Categoria>(selecaoQuery, new { categoriaId = categoriaId });
                return View(categoria);
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
        public async Task<IActionResult> AtualizarCategoria(Categoria categoria)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string atualizacaoQuery = "UPDATE categorias SET nome = @nome WHERE categoriaid = @categoriaId";
                    await _conexao.ExecuteAsync(atualizacaoQuery, categoria);
                    TempData["CategoriaAtualizada"] = $"Categoria {categoria.Nome} atualizada com sucesso";
                    return RedirectToAction(nameof(Index));
                }

                return View(categoria);

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
        public async Task<IActionResult> ExcluirCategoria(int categoriaId)
        {
            try
            {
                string exclusaoQuery = "DELETE FROM categorias WHERE categoriaid = @categoriaid";
                await _conexao.OpenAsync();
                await _conexao.ExecuteAsync(exclusaoQuery, new { categoriaid = categoriaId });
                TempData["CategoriaExcluida"] = $"Categoria excluida com sucesso";

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
