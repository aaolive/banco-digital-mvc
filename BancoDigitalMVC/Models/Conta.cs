using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BancoDigitalMVC.Models
{
    public class Conta
    {
        public int Id { get; set; }
        public string NumeroConta { get; set; } = Guid.NewGuid().ToString().Substring(0, 8);
        public decimal Saldo { get; set; } = 0;
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public ICollection<Transacao> Transacoes { get; set; } // Histórico


        public Conta()
        {
            Transacoes = new List<Transacao>();
        }
    }
}