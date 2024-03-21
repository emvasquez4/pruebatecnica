using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditCard.Modelos
{
    public class Transacciones
    {
        public int CodTransaccion { get; set; }
        public string? NumeroTarjeta { get; set; }
        public DateTime FechaTransaccion { get; set; }
        public string? Descripcion { get; set; }
        public string? Monto { get; set; }
        public string? Estado { get; set; }
        public string? Tipo { get; set; } //es cargo, es abono
    }
}
