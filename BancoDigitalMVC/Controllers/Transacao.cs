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
    public class TransacaoController : Controller
    {
        private readonly AppDbContext _db;

        public TransacaoController(AppDbContext db)
        {
            _db = db;
        }

        // Depósito
        [HttpPost]
        public IActionResult Depositar(int contaId, decimal valor)
        {
            var conta = _db.Contas.Find(contaId);
            conta.Saldo += valor;

            _db.Transacoes.Add(new Transacao
            {
                ContaId = contaId,
                Valor = valor,
                Tipo = "Deposito",
                Descricao = $"Depósito de R${valor}"
            });

            _db.SaveChanges();
            return RedirectToAction("Extrato", new { contaId });
        }
    }
}