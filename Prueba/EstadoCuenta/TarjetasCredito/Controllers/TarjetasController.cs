using CreditCard.Modelos;
using CreditCard.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CreditCard.Controllers
{
    public class TarjetasController : Controller
    {
        public readonly ITarjetaService tarjetas;

        public TarjetasController(ITarjetaService _tarjetas) { 
            tarjetas = _tarjetas;
        }

        [HttpGet]
        [Route("GetAllTarjetasCredito")]
        public async Task<ActionResult<List<TarjetaCredito>>> GetAllTarjetas()
        {
            try
            {
                var filtro = new Filtros();
                filtro.FiltroPrimario = "TODOS";
                filtro.FiltroSecundario = "";

                var listadoTarjetas = await tarjetas.GetAll(filtro);
                return Ok(listadoTarjetas);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetNumeroTarjeta")]
        public async Task<ActionResult<List<TarjetaCredito>>> GetNumTarjeta(string numtarjeta)
        {
            try
            {
                var filtro = new Filtros();
                filtro.FiltroPrimario = "NUMTARJETA";
                filtro.FiltroSecundario = numtarjeta;

                var listadoTarjetas = await tarjetas.GetNumCuenta(filtro);
                return Ok(listadoTarjetas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
