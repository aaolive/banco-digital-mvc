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
    public async Task<IActionResult> Register(Cliente cliente)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Seu código existente de cadastro
                _db.Clientes.Add(cliente);
                await _db.SaveChangesAsync();

                // Cria conta associada
                var conta = new Conta { ClienteId = cliente.Id };
                _db.Contas.Add(conta);
                await _db.SaveChangesAsync();

                // Redireciona para confirmação com dados do cliente
                return RedirectToAction("RegisterConfirmation", new { email = cliente.Email });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocorreu um erro ao cadastrar. Tente novamente.");
                // Log do erro (ex)
            }
        }
        else if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                // Log ou debug aqui
                Console.WriteLine(error.ErrorMessage);
            }
            return View(cliente);
        }
        return View(cliente);
    }

    [HttpGet]
    public IActionResult RegisterConfirmation(string email)
    {
        ViewBag.Email = email; // Opcional: mostra o email cadastrado
        return View();
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