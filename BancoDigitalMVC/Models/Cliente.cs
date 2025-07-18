using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BancoDigitalMVC.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; } // (Hash em produção!)
        public string CPF { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public Conta Conta { get; set; } // Relação 1:1 com Conta
    }
}