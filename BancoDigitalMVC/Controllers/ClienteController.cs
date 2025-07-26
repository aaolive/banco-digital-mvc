using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BancoDigitalMVC.Data;
using BancoDigitalMVC.Models;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null) return RedirectToAction("Login", "Auth");

            var cliente = _db.Clientes.Find(clienteId);
            var conta = _db.Contas.FirstOrDefault(c => c.ClienteId == clienteId);

            ViewBag.Cliente = cliente;
            ViewBag.Conta = conta;
            return View();
        }

        // GET: /Cliente/Perfil (Editar perfil)
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