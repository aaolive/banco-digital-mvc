using Microsoft.AspNetCore.Mvc;
using BancoDigitalMVC.Models;
using BancoDigitalMVC.Data;
using System.Linq;

namespace BancoDigitalMVC.Controllers
{
    [Route("[controller]")]
    public class TransacaoController : Controller
    {
        private readonly AppDbContext _db;

        public TransacaoController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Transacao/Deposito (Exibe o formulário)
        public IActionResult Depositar()
        {
            var contaId = HttpContext.Session.GetInt32("ContaId");
            if (contaId == null) return RedirectToAction("Login", "Auth");

            ViewBag.ContaId = contaId;
            return View();
        }

        // POST: /Transacao/Depositar (Processa o depósito)
        [HttpPost]
        public IActionResult Depositar(int contaId, decimal valor)
        {
            if (valor <= 0)
            {
                ViewBag.Error = "Valor inválido.";
                return View();
            }

            var conta = _db.Contas.Find(contaId);
            conta.Saldo += valor;

            _db.Transacoes.Add(new Transacao
            {
                ContaId = contaId,
                Valor = valor,
                Tipo = "Depósito",
                Descricao = $"Depósito de R${valor}"
            });

            _db.SaveChanges();
            return RedirectToAction("Extrato", new { contaId });
        }

        // GET: /Transacao/Saque (Exibe o formulário)
        public IActionResult Sacar()
        {
            var contaId = HttpContext.Session.GetInt32("ContaId");
            if (contaId == null) return RedirectToAction("Login", "Auth");

            ViewBag.ContaId = contaId;
            return View();
        }

        // POST: /Transacao/Sacar (Processa o saque)
        [HttpPost]
        public IActionResult Sacar(int contaId, decimal valor)
        {
            var conta = _db.Contas.Find(contaId);

            if (valor <= 0)
            {
                ViewBag.Error = "Valor inválido.";
                return View();
            }

            if (conta.Saldo < valor)
            {
                ViewBag.Error = "Saldo insuficiente.";
                return View();
            }

            conta.Saldo -= valor;

            _db.Transacoes.Add(new Transacao
            {
                ContaId = contaId,
                Valor = valor,
                Tipo = "Saque",
                Descricao = $"Saque de R${valor}"
            });

            _db.SaveChanges();
            return RedirectToAction("Extrato", new { contaId });
        }

        // GET: /Transacao/Transferencia (Exibe o formulário)
        public IActionResult Transferencia()
        {
            var contaId = HttpContext.Session.GetInt32("ContaId");
            if (contaId == null) return RedirectToAction("Login", "Auth");

            ViewBag.ContaId = contaId;
            return View();
        }

        // POST: /Transacao/Transferir (Processa a transferência)
        [HttpPost]
        public IActionResult Transferir(int contaOrigemId, string contaDestinoNumero, decimal valor)
        {
            var contaOrigem = _db.Contas.Find(contaOrigemId);
            var contaDestino = _db.Contas.FirstOrDefault(c => c.NumeroConta == contaDestinoNumero);

            if (contaDestino == null)
            {
                ViewBag.Error = "Conta destino não encontrada.";
                return View("Transferencia");
            }

            if (valor <= 0 || contaOrigem.Saldo < valor)
            {
                ViewBag.Error = "Valor inválido ou saldo insuficiente.";
                return View("Transferencia");
            }

            // Debita da conta origem
            contaOrigem.Saldo -= valor;
            _db.Transacoes.Add(new Transacao
            {
                ContaId = contaOrigemId,
                Valor = valor,
                Tipo = "Transferência (Enviada)",
                Descricao = $"Transferência para conta {contaDestinoNumero}"
            });

            // Credita na conta destino
            contaDestino.Saldo += valor;
            _db.Transacoes.Add(new Transacao
            {
                ContaId = contaDestino.Id,
                Valor = valor,
                Tipo = "Transferência (Recebida)",
                Descricao = $"Transferência da conta {contaOrigem.NumeroConta}"
            });

            _db.SaveChanges();
            ViewBag.Success = "Transferência realizada com sucesso!";
            return View("Transferencia");
        }

        // GET: /Transacao/Extrato (Lista transações)
        public IActionResult Extrato(int contaId)
        {
            var transacoes = _db.Transacoes
                .Where(t => t.ContaId == contaId)
                .OrderByDescending(t => t.Data)
                .ToList();

            var conta = _db.Contas.Find(contaId);
            ViewBag.Transacoes = transacoes;
            ViewBag.Conta = conta;
            return View();
        }
    }
}   