using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditCard.Modelos
{
    public class TarjetaCredito
    {
        public int CodTarjeta { get; set; }
        public string? NumeroTarjeta { get; set; } // Número de la tarjeta
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public double SaldoActual { get; set; }
        public double LimiteCredito { get; set; }
        public double SaldoDisponible { get; set; }
        public string? EstadoTarjeta { get; set; } //Estado de la tarjeta, Activo, Inactiva
        public double PorcentajeInteres { get; set; } //Interes aplicado a la tarjeta
        public double PorcentajeInteresMin { get; set; }
        [NotMapped]
        public List<Transacciones>? Transacciones { get; set; }

        [NotMapped]
        public double InteresBonificable { get; set; }
        [NotMapped]
        public double CuotaMinima { get; set; }
        [NotMapped]
        public double CuotaMaxima { get;set; }





    }
}
