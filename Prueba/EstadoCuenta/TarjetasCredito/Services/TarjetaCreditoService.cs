using CreditCard.ConexionDB;
using CreditCard.Modelos;
using Microsoft.EntityFrameworkCore;
using TarjetasCredito.Modelos;

namespace CreditCard.Services
{
    public interface ITarjetaService {
        Task<List<TarjetaCredito>> GetAll(Filtros filtro);
        Task<List<TarjetaCredito>> GetNumCuenta(Filtros filtro);
        Task<List<TarjetaCredito>> CalculoInteresBonificable(Filtros filtro);
        Task<string> UpdateSaldos(TarjetaCredito modelo, string tipotransaccion, double monto);
    }
    public class TarjetaCreditoService : ITarjetaService
    {

        private readonly Context dbContext;

        public TarjetaCreditoService(Context _dbContext) { 
            dbContext = _dbContext;
        }

        #region SELECT ALL
        public async Task<List<TarjetaCredito>> GetAll(Filtros filtro)
        {
            try
            {
               
                List<TarjetaCredito> Tarjetas = new List<TarjetaCredito>();

                Tarjetas = await dbContext.TarjetasCredito.ToListAsync();

                return Tarjetas;
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException("Error al consultar la base de datos: " + ex.Message, ex); 
            }
            catch (Exception e) {
                throw e;
            }
        }

        #endregion

        #region SELECT NUMERO DE TARJETA
        public async Task<List<TarjetaCredito>> GetNumCuenta(Filtros filtro)
        {
            try
            {
                double SaldoTotal = 0;

                List<TarjetaCredito> Tarjetas = new List<TarjetaCredito>();

                Tarjetas = await dbContext.TarjetasCredito.Where(x => x.NumeroTarjeta == filtro.FiltroSecundario).ToListAsync();
                if (Tarjetas.Count > 0) {
                    var transacciones = await dbContext.Transacciones.Where(y => y.NumeroTarjeta == filtro.FiltroSecundario && y.Tipo == "C").ToListAsync(); //PARA EL CASO C SIGNIFICA CARGO O COMPRA 
                    if (transacciones.Count > 0) { 
                        SaldoTotal = transacciones.Sum(x => Convert.ToDouble(x.Monto));
                    }

                    Tarjetas[0].SaldoActual = Tarjetas[0].SaldoActual == null ? 0 : Tarjetas[0].SaldoActual;
                    Tarjetas[0].PorcentajeInteres = Tarjetas[0].PorcentajeInteres == null ? 0 : Tarjetas[0].PorcentajeInteres;
                    Tarjetas[0].InteresBonificable = SaldoTotal * (Tarjetas[0].PorcentajeInteres / 100); //ASUMIENDO QUE EL CAMPO PORCENTAJE SE GUARDA COMO PORCENTAJE

                    Tarjetas[0].SaldoActual = Tarjetas[0].SaldoActual == null ? 0 : Tarjetas[0].SaldoActual;
                    Tarjetas[0].PorcentajeInteresMin = Tarjetas[0].PorcentajeInteresMin == null ? 0 : Tarjetas[0].PorcentajeInteresMin;
                    Tarjetas[0].CuotaMinima = SaldoTotal * (Tarjetas[0].PorcentajeInteresMin / 100); //ASUMIENDO QUE EL CAMPO PORCENTAJE SE GUARDA COMO PORCENTAJE AL CALCULO DE CUOTA MIN

                    Tarjetas[0].SaldoTotal = SaldoTotal; // monto total a pagar

                    Tarjetas[0].MontoTotalInteres = Tarjetas[0].InteresBonificable + SaldoTotal; // ([Saldo Total] + [Interés Bonificable]) 
                }

                

                return Tarjetas;
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException("Error al consultar la base de datos: " + ex.Message, ex);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion


        #region CALCULAR INTERES BONIFICABLE
        public async Task<List<TarjetaCredito>> CalculoInteresBonificable(Filtros filtro)
        {
            try
            {

                List<TarjetaCredito> Tarjetas = new List<TarjetaCredito>();

                Tarjetas = await dbContext.TarjetasCredito.Where(x => x.NumeroTarjeta == filtro.FiltroSecundario).ToListAsync();
                Tarjetas[0].SaldoActual = Tarjetas[0].SaldoActual == null ? 0 : Tarjetas[0].SaldoActual;
                Tarjetas[0].PorcentajeInteres = Tarjetas[0].PorcentajeInteres == null ? 0 : Tarjetas[0].PorcentajeInteres;
                Tarjetas[0].InteresBonificable = Tarjetas[0].SaldoActual * (Tarjetas[0].PorcentajeInteres/100); //ASUMIENDO QUE EL CAMPO PORCENTAJE SE GUARDA COMO PORCENTAJE

                return Tarjetas;
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException("Error al consultar la base de datos: " + ex.Message, ex);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region CALCULAR INTERES BONIFICABLE MIN
        public async Task<List<TarjetaCredito>> CalculoInteresBonificableMin(Filtros filtro)
        {
            try
            {

                List<TarjetaCredito> Tarjetas = new List<TarjetaCredito>();

                Tarjetas = await dbContext.TarjetasCredito.Where(x => x.NumeroTarjeta == filtro.FiltroSecundario).ToListAsync();
                Tarjetas[0].SaldoActual = Tarjetas[0].SaldoActual == null ? 0 : Tarjetas[0].SaldoActual;
                Tarjetas[0].PorcentajeInteresMin = Tarjetas[0].PorcentajeInteresMin == null ? 0 : Tarjetas[0].PorcentajeInteresMin;
                Tarjetas[0].InteresBonificable = Tarjetas[0].SaldoActual * (Tarjetas[0].PorcentajeInteres / 100); //ASUMIENDO QUE EL CAMPO PORCENTAJE SE GUARDA COMO PORCENTAJE

                return Tarjetas;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region ACTUALIZAR NUMEROTARJETA (PAGO/COMPRA)
        public async Task<string> UpdateSaldos(TarjetaCredito modelo, string tipotransaccion, double monto)
        {
            try
            {
                //DEPENDE DEL TIPO DE TRANSACCION ABONO O CARGO
                var tarjeta = await dbContext.TarjetasCredito.Where(x => x.NumeroTarjeta == modelo.NumeroTarjeta).ToListAsync();
                if (tarjeta == null)
                {
                    throw new Exception("La tarjeta no existe o no se pudo obtener información.");
                }
                if (tipotransaccion == "A") //ABONO
                {
                    tarjeta[0].SaldoActual = tarjeta[0].SaldoActual - monto;
                }
                if(tipotransaccion == "C") { //CARGO
                    tarjeta[0].SaldoActual = tarjeta[0].SaldoActual + monto;
                }
              
                dbContext.SaveChanges();
                return "Exito";
            }
            catch (Exception e)
            {
                throw new UpdateSaldosException("Error al actualizar los saldos de la tarjeta.");
            }
        }

        #endregion
    }
}
