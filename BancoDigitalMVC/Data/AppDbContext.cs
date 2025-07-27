using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BancoDigitalMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BancoDigitalMVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Conta> Contas { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=BancoDigital.db");
    }
}