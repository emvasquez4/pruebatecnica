using Microsoft.EntityFrameworkCore;
using CreditCard.Modelos;

namespace CreditCard.ConexionDB
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        

        public DbSet<TarjetaCredito> TarjetasCredito { get; set; }
        public DbSet<Transacciones> Transacciones { get; set; }

        protected override void OnModelCreating(ModelBuilder dbBuilder) 
        {
            dbBuilder.Entity<TarjetaCredito>().HasKey(d => new { d.CodTarjeta, d.NumeroTarjeta });
            
            dbBuilder.Entity<Transacciones>().HasKey(c => new { c.CodTransaccion });
        }
    }
}