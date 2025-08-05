using Microsoft.AspNetCore.Mvc;
using BancoDigitalMVC.Models;
using BancoDigitalMVC.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> Register(ClienteDTO cliente)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // converte DTO para entidade
                var clienteNovoParaCadastro = new Cliente();
                clienteNovoParaCadastro.Nome = cliente.Nome;
                clienteNovoParaCadastro.Email = cliente.Email;
                clienteNovoParaCadastro.Senha = cliente.Senha; // Lembre-se de hashear a senha em produção!
                clienteNovoParaCadastro.CPF = cliente.CPF;


                // Seu código existente de cadastro
                _db.Clientes.Add(clienteNovoParaCadastro);
                await _db.SaveChangesAsync();

                // Cria conta associada
                var conta = new Conta { ClienteId = clienteNovoParaCadastro.Id };
                _db.Contas.Add(conta);
                await _db.SaveChangesAsync();

                // Redireciona para confirmação com dados do cliente
                return RedirectToAction("RegisterConfirmation", new { email = clienteNovoParaCadastro.Email });
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
        // Verifique se o serviço de sessão está disponível
        if (HttpContext.Session == null)
        {
            throw new InvalidOperationException("Session service not available!");
        }

        var cliente = _db.Clientes
            .Include(c => c.Conta) // Carrega a conta associada
            .FirstOrDefault(c => c.Email == email && c.Senha == senha);

        if (cliente == null)
        {
            ModelState.AddModelError("", "E-mail ou senha incorretos.");
            return View();
        }

        // Armazena informações na sessão
        HttpContext.Session.SetInt32("ClienteId", cliente.Id);
        HttpContext.Session.SetString("ClienteNome", cliente.Nome);
        HttpContext.Session.SetInt32("ContaId", cliente.Conta.Id);

        return RedirectToAction("Index", "Cliente");
    }

    // GET: /Auth/Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}