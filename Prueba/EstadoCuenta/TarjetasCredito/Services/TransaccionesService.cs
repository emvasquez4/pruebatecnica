using CreditCard.ConexionDB;
using CreditCard.Modelos;
using Microsoft.EntityFrameworkCore;

namespace CreditCard.Services
{
    public interface ITransaccionesService {
        Task<List<Transacciones>> GetAll(Filtros filtro);
        Task<List<Transacciones>> GetNumCuenta(Filtros filtro);

        Task<string> InsertTransacciones(Transacciones modelo);
    }
    public class TransaccionesService : ITransaccionesService
    {

        private readonly Context dbContext;

        public TransaccionesService(Context _dbContext) { 
            dbContext = _dbContext;
        }

        #region SELECT ALL
        public async Task<List<Transacciones>> GetAll(Filtros filtro)
        {
            try
            {
               
                List<Transacciones> Transacciones = new List<Transacciones>();

                Transacciones = await dbContext.Transacciones.ToListAsync();

                return Transacciones;
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

        #region SELECT NUMERO DE TARJETA ULTIMOS DOS MESES
        public async Task<List<Transacciones>> GetNumCuenta(Filtros filtro)
        {
            try
            {

                List<Transacciones> Transacciones = new List<Transacciones>();
                List<Transacciones> TransaccionesCuenta = new List<Transacciones>();

                Transacciones = await dbContext.Transacciones.Where(x => x.NumeroTarjeta == filtro.FiltroSecundario).ToListAsync();

                var hoy = DateTime.Today;
                var primerDiaMes = new DateTime(hoy.Year, hoy.Month, 1);
                var mesPasado = primerDiaMes.AddMonths(-1);

                var trxMesActual = Transacciones.Where(x => x.FechaTransaccion >= primerDiaMes && x.FechaTransaccion <= hoy).ToList(); //Transacciones del mes actual
                var trxMesAnterior = Transacciones.Where(x => x.FechaTransaccion >= mesPasado && x.FechaTransaccion < primerDiaMes).ToList(); //Transacciones del mes anterior

                if (trxMesAnterior.Count > 0 && trxMesActual.Count > 0)
                {
                    TransaccionesCuenta = trxMesActual.Concat(trxMesAnterior).ToList();
                }
                else if (trxMesActual.Count > 0 && trxMesAnterior.Count == 0)
                {
                    TransaccionesCuenta = trxMesActual;
                }
                else if (trxMesActual.Count == 0 && trxMesAnterior.Count > 0)
                {
                    TransaccionesCuenta = trxMesAnterior;
                }

                return TransaccionesCuenta;
            }
            catch (Exception e)
            {
                throw new Exception("Error al obtener las transacciones de la cuenta");
            }
        }

        #endregion

        #region SELECT HISTORIAL DE TRANSACCIONES DEL MES
        public async Task<List<Transacciones>> GetHistorialTransacciones(Filtros filtro)
        {
            try
            {

                List<Transacciones> Transacciones = new List<Transacciones>();
                List<Transacciones> TransaccionesCuenta = new List<Transacciones>();

                Transacciones = await dbContext.Transacciones.Where(x => x.NumeroTarjeta == filtro.FiltroSecundario).ToListAsync();

                var hoy = DateTime.Today;
                var primerDiaMes = new DateTime(hoy.Year, hoy.Month, 1);
                var mesPasado = primerDiaMes.AddMonths(-1);

                var trxMesActual = Transacciones.Where(x => x.FechaTransaccion >= primerDiaMes && x.FechaTransaccion <= hoy).OrderByDescending(X => X.FechaTransaccion).ToList(); //Transacciones del mes actual
                


                return TransaccionesCuenta;
            }
            catch (Exception e)
            {
                throw new Exception("Error al obtener las transacciones de la cuenta");
            }
        }

        #endregion

        #region INSERT TRANSACCION (PAGO/COMPRA)
        public async Task<string> InsertTransacciones(Transacciones modelo)
        {
            try
            {
                //DEPENDE DEL TIPO DE TRANSACCION ABONO O CARGO

                modelo.Descripcion = modelo.Descripcion != null ? modelo.Descripcion : "no data";
                modelo.Estado = modelo.Estado != null ? modelo.Estado : "A";

                dbContext.Transacciones.Add(modelo);
                dbContext.SaveChanges();
                return "Exito";
            }
            catch (Exception e)
            {
                throw new Exception("Error al insertar las transacciones de la cuenta");
            }
        }

        #endregion
    }
}
