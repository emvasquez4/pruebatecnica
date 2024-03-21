using System.ComponentModel.DataAnnotations.Schema;

namespace CreditCard.Modelos
{
    [NotMapped]
    public class Filtros
    {
        public string? FiltroPrimario { get; set; }
        public string? FiltroSecundario { get; set; }
    }
}
