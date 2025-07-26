using Microsoft.AspNetCore.Mvc;
using BancoDigitalMVC.Models;
using BancoDigitalMVC.Data;
using System.Linq;

namespace BancoDigitalMVC.Controllers;
public class AuthController : Controller
{
    private readonly AppDbContext _db;

    public AuthController(AppDbContext db)
    {
        _db = db;
    }

    // GET: /Auth/Register (Exibe o formulário)
    public IActionResult Register()
    {
        return View();
    }

    // POST: /Auth/Register (Processa o cadastro)
    [HttpPost]
    public IActionResult Register(Cliente cliente)
    {
        if (ModelState.IsValid)
        {
            // Verifica se o CPF ou E-mail já existem
            if (_db.Clientes.Any(c => c.CPF == cliente.CPF || c.Email == cliente.Email))
            {
                ModelState.AddModelError("", "CPF ou E-mail já cadastrados.");
                return View(cliente);
            }

            _db.Clientes.Add(cliente);
            _db.SaveChanges();

            // Cria uma conta para o cliente
            var conta = new Conta { ClienteId = cliente.Id };
            _db.Contas.Add(conta);
            _db.SaveChanges();

            return RedirectToAction("Login");
        }
        return View(cliente);
    }

    // GET: /Auth/Login (Exibe o formulário)
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Auth/Login (Processa o login)
    [HttpPost]
    public IActionResult Login(string email, string senha)
    {
        var cliente = _db.Clientes.FirstOrDefault(c => c.Email == email && c.Senha == senha);

        if (cliente == null)
        {
            ModelState.AddModelError("", "E-mail ou senha incorretos.");
            return View();
        }

        // Simples autenticação (em produção, use Identity!)
        HttpContext.Session.SetInt32("ClienteId", cliente.Id);
        return RedirectToAction("Index", "Cliente");
    }

    // GET: /Auth/Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}