using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PasajesAeropuerto.Data;
using PasajesAeropuerto.Entities;

namespace PasajesAeropuerto.Controllers
{
    public class HomeController : Controller
    {
        private readonly PasajesAeropuertoContext _context;

        public HomeController(PasajesAeropuertoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await CargarListasAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgregarPasaje(
            string Nombre,
            string Apellido,
            string Dni,
            string Email,
            int DestinoId,
            int VueloId,
            int AvionId,
            int CantPersonas)
        {
            if (string.IsNullOrWhiteSpace(Nombre) ||
                string.IsNullOrWhiteSpace(Apellido) ||
                string.IsNullOrWhiteSpace(Dni) ||
                string.IsNullOrWhiteSpace(Email))
            {
                TempData["Error"] = "Completá todos los datos del pasajero.";
                await CargarListasAsync();
                return View("Index");
            }

            if (CantPersonas < 1 || CantPersonas > 10)
            {
                TempData["Error"] = "La cantidad de personas debe ser entre 1 y 10.";
                await CargarListasAsync();
                return View("Index");
            }

            var destino = await _context.Destinos.FindAsync(DestinoId);
            if (destino is null)
            {
                TempData["Error"] = "Destino no válido.";
                await CargarListasAsync();
                return View("Index");
            }

            if (!await _context.Vuelos.AnyAsync(v => v.Id == VueloId))
            {
                TempData["Error"] = "Vuelo no válido.";
                await CargarListasAsync();
                return View("Index");
            }

            if (!await _context.Aviones.AnyAsync(a => a.Id == AvionId))
            {
                TempData["Error"] = "Avión no válido.";
                await CargarListasAsync();
                return View("Index");
            }

            var pasajero = new Pasajero
            {
                Nombre = Nombre.Trim(),
                Apellido = Apellido.Trim(),
                Dni = Dni.Trim(),
                Email = Email.Trim()
            };
            _context.Pasajeros.Add(pasajero);

            var reserva = new Reserva
            {
                CantPersonas = CantPersonas,
                FechaEmision = DateTime.Now,
                TotalCalculado = destino.PrecioBase * CantPersonas
            };
            _context.Reservas.Add(reserva);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                $"Pasaje emitido. Reserva #{reserva.Id} — {CantPersonas} persona(s) — Total: {reserva.TotalCalculado:C}";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListasAsync()
        {
            var vuelos = await _context.Vuelos
                .OrderBy(v => v.FechaSalida)
                .Select(v => new
                {
                    v.Id,
                    Texto = v.Numero + " - " + v.Aerolinea + " (" + v.FechaSalida.ToString("dd/MM/yyyy") + ")"
                })
                .ToListAsync();

            ViewBag.Vuelos = new SelectList(vuelos, "Id", "Texto");

            ViewBag.Destinos = new SelectList(
                await _context.Destinos.OrderBy(d => d.Nombre).ToListAsync(),
                "Id",
                "Nombre");

            var aviones = await _context.Aviones
                .OrderBy(a => a.Modelo)
                .Select(a => new { a.Id, Texto = a.Modelo + " (" + a.Matricula + ")" })
                .ToListAsync();

            ViewBag.Aviones = new SelectList(aviones, "Id", "Texto");
        }
    }
}
