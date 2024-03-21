using CreditCardsWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace CreditCardsWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        readonly Uri baseAddress = new("https://localhost:44360/api");
        private readonly HttpClient _httpClient;

        public HomeController()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = baseAddress
            };
        }

        [HttpGet]
        public IActionResult GenerarExcel(int id)
        {
            try
            {
                HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/TarjetaCredito/GetTarjetaCredito?id=" + id).Result;

                string data = response.Content.ReadAsStringAsync().Result;
                List<TarjetaCredito> tarjetaList = JsonConvert.DeserializeObject<List<TarjetaCredito>>(data);

                tarjetaList[0].SaldoActual = 0;
                tarjetaList[0].ComprasPrevio = 0;
                tarjetaList[0].ComprasActual = 0;
                tarjetaList[0].NombreMesPrev = DateTime.Now.AddDays(-DateTime.Now.Day).ToString("MMMM", new CultureInfo("es-ES"));
                tarjetaList[0].NombreMesAct = DateTime.Now.ToString("MMMM", new CultureInfo("es-ES"));
                tarjetaList[0].NumeroTarjetaMask = $"{tarjetaList[0].NumeroTarjeta[..4]} **** **** {tarjetaList[0].NumeroTarjeta[12..]}";

                foreach (var item in tarjetaList[0].Transacciones)
                {
                    if (item.AbonoCargo == "C")
                    {
                        tarjetaList[0].SaldoActual += item.Monto;
                        if (item.Fecha.Year == DateTime.Now.Year && item.Fecha.Month == DateTime.Now.Month)
                        {
                            tarjetaList[0].ComprasActual += item.Monto;
                        }
                        else if (item.Fecha.Month == DateTime.Now.AddDays(-DateTime.Now.Day).Month && item.Fecha.Year == DateTime.Now.AddDays(-DateTime.Now.Day).Year)
                        {
                            tarjetaList[0].ComprasPrevio += item.Monto;
                        }
                    }
                    else
                    {
                        tarjetaList[0].SaldoActual -= item.Monto;
                    }
                    item.FechaText = item.Fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    item.NumeroAutorizacion = item.Id.ToString().PadLeft(6, '0');
                    item.MontoText = item.Monto.ToString("C");
                }

                tarjetaList[0].SaldoDisponible = tarjetaList[0].Limite - tarjetaList[0].SaldoActual;
                tarjetaList[0].InteresBonificable = tarjetaList[0].ComprasActual * tarjetaList[0].PorcInteres / 100;
                tarjetaList[0].Transacciones =
                [
                    .. tarjetaList[0].Transacciones.Where(s =>
                                    s.AbonoCargo == "C" && s.Fecha.Year == DateTime.Now.Year && s.Fecha.Month == DateTime.Now.Month).OrderByDescending(s => s.Fecha),
                ];

                var numrow = 1;


                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Compras");
                worksheet.Cell(numrow, 1).Value = "Número de Autorización";
                worksheet.Cell(numrow, 2).Value = "Fecha";
                worksheet.Cell(numrow, 3).Value = "Descripción";
                worksheet.Cell(numrow, 4).Value = "Monto";

                foreach (var item in tarjetaList[0].Transacciones)
                {
                    numrow++;
                    worksheet.Cell(numrow, 1).Value = item.NumeroAutorizacion;
                    worksheet.Cell(numrow, 2).Value = item.Fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    worksheet.Cell(numrow, 3).Value = item.Descripcion;
                    worksheet.Cell(numrow, 4).Value = item.Monto.ToString("C");
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return File(content, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Compras.xlsx");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
