using Microsoft.AspNetCore.Mvc;
using SistemaNaoOtimizado.Context;
using SistemaNaoOtimizado.Models;
using System.Diagnostics;

namespace SistemaNaoOtimizado.Controllers
{
    public class ProdutosLentoController : Controller
    {
        private readonly AppDbContext _context;

        public ProdutosLentoController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(bool iniciarTeste = false)
        {
            if (!iniciarTeste)
            {
                ViewBag.Status = "Aguardando Início...";
                return View(new List<Produto>());
            }

            var sw = new Stopwatch();
            sw.Start();

            var listaProdutos = _context.Produtos.ToList();

            foreach (var item in listaProdutos)
            {
                item.Categoria = _context.Categorias.Find(item.CategoriaId);
                Thread.Sleep(5);
            }

            sw.Stop();

            ViewBag.Tempo = sw.ElapsedMilliseconds + " ms";
            ViewBag.Tipo = "Versão Lenta (Não Otimizada)";
            ViewBag.TesteExecutado = true;

            return View(listaProdutos);
        }

        [HttpPost]
        public IActionResult GerarDadosMassivos()
        {
            if (_context.Produtos.Any())
            {
                TempData["Mensagem"] = "O banco já possui dados!";
                return RedirectToAction("Index");
            }

            var categorias = new List<Categoria>();
            for (int i = 1; i <= 50; i++) categorias.Add(new Categoria { Nome = $"Categoria {i}" });
            _context.Categorias.AddRange(categorias);
            _context.SaveChanges();

            var produtos = new List<Produto>();
            var random = new Random();
            var listaCats = _context.Categorias.ToList();

            for (int i = 1; i <= 2000; i++)
            {
                produtos.Add(new Produto
                {
                    Nome = $"Produto Teste {i}",
                    Preco = random.Next(10, 500),
                    CategoriaId = listaCats[random.Next(listaCats.Count)].Id
                });
            }

            _context.Produtos.AddRange(produtos);
            _context.SaveChanges();

            TempData["Mensagem"] = "Sucesso! 2.000 produtos gerados.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult LimparDados()
        {
            _context.Produtos.RemoveRange(_context.Produtos);
            _context.Categorias.RemoveRange(_context.Categorias);
            _context.SaveChanges();

            TempData["Mensagem"] = "Banco limpo com sucesso!";
            return RedirectToAction("Index");
        }
    }
}