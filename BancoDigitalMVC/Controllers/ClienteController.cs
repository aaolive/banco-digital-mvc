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

        // Cadastro de cliente
        [HttpPost]
        public IActionResult Cadastrar(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _db.Clientes.Add(cliente);
                _db.SaveChanges();

                // Cria uma conta para o cliente
                var conta = new Conta { ClienteId = cliente.Id };
                _db.Contas.Add(conta);
                _db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            return View("Register", cliente);
        }
    }
}