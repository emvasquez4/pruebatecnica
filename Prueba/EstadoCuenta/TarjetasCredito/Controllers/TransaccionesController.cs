using CreditCard.Modelos;
using CreditCard.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CreditCard.Controllers
{
    public class TransaccionesController : Controller
    {
        public readonly ITransaccionesService transacciones;
        public readonly ITarjetaService tarjeta;

        public TransaccionesController(ITransaccionesService _transacciones, ITarjetaService _tarjeta) {
            transacciones = _transacciones;
            tarjeta = _tarjeta;
        }

        [HttpGet]
        [Route("GetAllTransacciones")]
        public async Task<ActionResult<List<Transacciones>>> GetAllTransacciones()
        {
            try
            {
                var filtro = new Filtros();
                filtro.FiltroPrimario = "TODOS";
                filtro.FiltroSecundario = "";

                var listadoTransacciones = await transacciones.GetAll(filtro);
                return Ok(listadoTransacciones);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetTrxNumeroTarjetaDosMeses")]
        public async Task<ActionResult<List<Transacciones>>> GetNumTarjeta(string numtarjeta)
        {
            try
            {
                var filtro = new Filtros();
                filtro.FiltroPrimario = "NUMTARJETA";
                filtro.FiltroSecundario = numtarjeta;

                var listadoTransacciones = await transacciones.GetNumCuenta(filtro);
                return Ok(listadoTransacciones);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("InsertTransaccionesPago")]
        public async Task<ActionResult<string>> InsertTransaccion(Transacciones transaccion)
        {
            try
            {
                var filtro = new Filtros();
                filtro.FiltroPrimario = "NUMTARJETA";
                filtro.FiltroSecundario = transaccion.NumeroTarjeta;
                //VALIDAMOS SI TIENE DISPONIBLE
                var datostarjeta = await tarjeta.GetNumCuenta(filtro);
                if (datostarjeta.Count > 0)
                { //TARJETA EXISTENTE
                   
                         //ABONO
                        
                            //SE ACTUALIZA EL SALDO ACTUAL 
                            tarjeta.UpdateSaldos(datostarjeta[0], "A", Convert.ToDouble(transaccion.Monto));

                            //SE REGISTRA LA TRANSACCION
                            transacciones.InsertTransacciones(transaccion);

                            return Ok("PAGO REALIZADO CORRECTAMENTE");
                        
                    
                }
                else {
                    return Ok("NUMERO DE TARJETA INEXISTENTE");
                }
               
               
            }
            catch (NullReferenceException ex)
            {
                return BadRequest("Error: La tarjeta no existe o no se pudo obtener información.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }


        [HttpPut]
        [Route("InsertTransaccionesCompra")]
        public async Task<ActionResult<string>> InsertTransaccionCompra(Transacciones transaccion)
        {
            try
            {
                var filtro = new Filtros();
                filtro.FiltroPrimario = "NUMTARJETA";
                filtro.FiltroSecundario = transaccion.NumeroTarjeta;
                //VALIDAMOS SI TIENE DISPONIBLE
                var datostarjeta = await tarjeta.GetNumCuenta(filtro);
                if (datostarjeta.Count > 0)
                { //TARJETA EXISTENTE

                        if (datostarjeta[0].SaldoDisponible >= Convert.ToDouble(transaccion.Monto))
                        {
                            //SE ACTUALIZA EL SALDO ACTUAL
                            tarjeta.UpdateSaldos(datostarjeta[0], "C", Convert.ToDouble(transaccion.Monto));


                            //SE REGISTRA LA TRANSACCION
                            transacciones.InsertTransacciones(transaccion);

                            return Ok("PAGO REALIZADO CORRECTAMENTE");
                        }
                        else
                        {
                            return Ok("COMPRA MAYOR A DISPONIBLE");
                        }

                }
                else
                {
                    return Ok("NUMERO DE TARJETA INEXISTENTE");
                }


            }
            catch (NullReferenceException ex)
            {
                return BadRequest("Error: La tarjeta no existe o no se pudo obtener información.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
