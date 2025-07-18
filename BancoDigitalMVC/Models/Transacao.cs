using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BancoDigitalMVC.Models
{
    public class Transacao
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; } // "Deposito", "Saque", "Transferencia"
        public DateTime Data { get; set; } = DateTime.Now;
        public string Descricao { get; set; }

        public int ContaId { get; set; }
        public Conta Conta { get; set; }
    }
}