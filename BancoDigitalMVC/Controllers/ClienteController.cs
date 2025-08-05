using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BancoDigitalMVC.Data;
using BancoDigitalMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BancoDigitalMVC.Controllers
{
    [Route("[controller]")]
    public class ClienteController : Controller
    {
        private readonly AppDbContext _db;

        public ClienteController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Cliente/Index (Dashboard)
        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null) return RedirectToAction("Login", "Auth");

            // CARREGUE AS TRANSAÇÕES INCLUSIVE
            var cliente = _db.Clientes
                .Include(c => c.Conta)
                .ThenInclude(c => c.Transacoes) // IMPORTANTE!
                .FirstOrDefault(c => c.Id == clienteId);

            // Debugging para checar se o cliente e a conta foram encontrados
            Console.WriteLine($"Cliente encontrado: {cliente != null}");
            Console.WriteLine($"Conta associada: {cliente?.Conta != null}");
            Console.WriteLine($"Transações: {cliente?.Conta?.Transacoes?.Count ?? 0}");

            if (cliente == null) return RedirectToAction("Login", "Auth");

            ViewBag.Cliente = cliente;
            ViewBag.Conta = cliente.Conta;

            return View();
        }

        // GET: /Cliente/Perfil (Editar perfil)
        [HttpGet("Perfil")]
        public IActionResult Perfil()
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null) return RedirectToAction("Login", "Auth");

            var cliente = _db.Clientes.Find(clienteId);
            return View(cliente);
        }

        // POST: /Cliente/Atualizar (Salvar alterações)
        [HttpPost]
        public IActionResult Atualizar(Cliente clienteAtualizado)
        {
            if (ModelState.IsValid)
            {
                var cliente = _db.Clientes.Find(clienteAtualizado.Id);
                cliente.Nome = clienteAtualizado.Nome;
                cliente.Email = clienteAtualizado.Email;

                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Perfil", clienteAtualizado);
        }
    }
}