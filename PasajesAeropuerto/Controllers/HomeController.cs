using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PasajesAeropuerto.Data;
using PasajesAeropuerto.Entities;
using PasajesAeropuerto.Services;

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
        public async Task<IActionResult> VerPrecios(
            int DestinoId,
            int VueloId,
            string Clase,
            int CantPersonas,
            bool EquipajeValija,
            bool EquipajeAdicional,
            string? Nombre,
            string? Apellido,
            string? Dni,
            string? Email,
            int AvionId)
        {
            var error = await ValidarDatosViajeAsync(DestinoId, VueloId, CantPersonas);
            if (error is not null)
            {
                ViewBag.Error = error;
                await RestaurarFormularioAsync(DestinoId, VueloId, Clase, CantPersonas, EquipajeValija, EquipajeAdicional, Nombre, Apellido, Dni, Email, AvionId);
                return View("Index");
            }

            var destino = await _context.Destinos.FindAsync(DestinoId);
            var vuelo = await _context.Vuelos.FindAsync(VueloId);
            var clase = string.IsNullOrWhiteSpace(Clase) ? "Economica" : Clase;

            var tiposEquipaje = await ObtenerTiposEquipajeAsync();
            ViewBag.Simulacion = CalculadorPrecioPasaje.Calcular(
                destino!, vuelo!, clase, CantPersonas, EquipajeValija, EquipajeAdicional, tiposEquipaje);
            ViewBag.PreciosVistos = true;

            await RestaurarFormularioAsync(DestinoId, VueloId, clase, CantPersonas, EquipajeValija, EquipajeAdicional, Nombre, Apellido, Dni, Email, AvionId);
            return View("Index");
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
            string Clase,
            int CantPersonas,
            bool EquipajeValija,
            bool EquipajeAdicional,
            bool PreciosVistos)
        {
            await RestaurarFormularioAsync(DestinoId, VueloId, Clase, CantPersonas, EquipajeValija, EquipajeAdicional, Nombre, Apellido, Dni, Email, AvionId);

            if (!PreciosVistos)
            {
                ViewBag.Error = "Primero debés hacer clic en «Ver precios».";
                return View("Index");
            }

            if (string.IsNullOrWhiteSpace(Nombre) ||
                string.IsNullOrWhiteSpace(Apellido) ||
                string.IsNullOrWhiteSpace(Dni) ||
                string.IsNullOrWhiteSpace(Email))
            {
                ViewBag.Error = "Completá todos los datos del pasajero.";
                await RecalcularYMostrarPreciosAsync(DestinoId, VueloId, Clase, CantPersonas, EquipajeValija, EquipajeAdicional);
                return View("Index");
            }

            var errorViaje = await ValidarDatosViajeAsync(DestinoId, VueloId, CantPersonas);
            if (errorViaje is not null)
            {
                ViewBag.Error = errorViaje;
                return View("Index");
            }

            if (!await _context.Aviones.AnyAsync(a => a.Id == AvionId))
            {
                ViewBag.Error = "Avión no válido.";
                await RecalcularYMostrarPreciosAsync(DestinoId, VueloId, Clase, CantPersonas, EquipajeValija, EquipajeAdicional);
                return View("Index");
            }

            var destino = await _context.Destinos.FindAsync(DestinoId);
            var vuelo = await _context.Vuelos.FindAsync(VueloId);
            var clase = string.IsNullOrWhiteSpace(Clase) ? "Economica" : Clase;
            var tiposEquipaje = await ObtenerTiposEquipajeAsync();
            var simulacion = CalculadorPrecioPasaje.Calcular(
                destino!, vuelo!, clase, CantPersonas, EquipajeValija, EquipajeAdicional, tiposEquipaje);

            var pasajero = new Pasajero
            {
                Nombre = Nombre.Trim(),
                Apellido = Apellido.Trim(),
                Dni = Dni.Trim(),
                Email = Email.Trim()
            };
            _context.Pasajeros.Add(pasajero);

            pasajero.Equipajes.Add(new Equipaje
            {
                TipoEquipajeId = TipoEquipaje.IdMano,
                Cantidad = 1
            });
            if (EquipajeValija)
            {
                pasajero.Equipajes.Add(new Equipaje
                {
                    TipoEquipajeId = TipoEquipaje.IdValija,
                    Cantidad = 1
                });
            }
            if (EquipajeAdicional)
            {
                pasajero.Equipajes.Add(new Equipaje
                {
                    TipoEquipajeId = TipoEquipaje.IdAdicional,
                    Cantidad = 1
                });
            }

            var reserva = new Reserva
            {
                CantPersonas = CantPersonas,
                FechaEmision = DateTime.Now,
                TotalCalculado = simulacion.Total
            };
            _context.Reservas.Add(reserva);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                $"Pasaje emitido. Reserva #{reserva.Id} — {CantPersonas} persona(s) — Total: {reserva.TotalCalculado:C}";
            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> ValidarDatosViajeAsync(int destinoId, int vueloId, int cantPersonas)
        {
            if (destinoId <= 0 || vueloId <= 0)
            {
                return "Seleccioná destino y vuelo.";
            }

            if (cantPersonas < 1 || cantPersonas > 10)
            {
                return "La cantidad de personas debe ser entre 1 y 10.";
            }

            if (!await _context.Destinos.AnyAsync(d => d.Id == destinoId))
            {
                return "Destino no válido.";
            }

            if (!await _context.Vuelos.AnyAsync(v => v.Id == vueloId))
            {
                return "Vuelo no válido.";
            }

            return null;
        }

        private async Task RecalcularYMostrarPreciosAsync(
            int destinoId, int vueloId, string clase, int cantPersonas,
            bool equipajeValija, bool equipajeAdicional)
        {
            var destino = await _context.Destinos.FindAsync(destinoId);
            var vuelo = await _context.Vuelos.FindAsync(vueloId);
            if (destino is null || vuelo is null) return;

            var tiposEquipaje = await ObtenerTiposEquipajeAsync();
            ViewBag.Simulacion = CalculadorPrecioPasaje.Calcular(
                destino, vuelo, string.IsNullOrWhiteSpace(clase) ? "Economica" : clase,
                cantPersonas, equipajeValija, equipajeAdicional, tiposEquipaje);
            ViewBag.PreciosVistos = true;
        }

        private async Task<List<TipoEquipaje>> ObtenerTiposEquipajeAsync()
        {
            return await _context.TiposEquipaje.OrderBy(t => t.Id).ToListAsync();
        }

        private async Task RestaurarFormularioAsync(
            int destinoId, int vueloId, string? clase, int cantPersonas,
            bool equipajeValija, bool equipajeAdicional,
            string? nombre, string? apellido, string? dni, string? email, int avionId)
        {
            ViewBag.Clase = string.IsNullOrWhiteSpace(clase) ? "Economica" : clase;
            ViewBag.CantPersonas = cantPersonas;
            ViewBag.EquipajeValija = equipajeValija;
            ViewBag.EquipajeAdicional = equipajeAdicional;
            ViewBag.Nombre = nombre ?? "";
            ViewBag.Apellido = apellido ?? "";
            ViewBag.Dni = dni ?? "";
            ViewBag.Email = email ?? "";
            await CargarListasAsync(
                destinoId > 0 ? destinoId : null,
                vueloId > 0 ? vueloId : null,
                avionId > 0 ? avionId : null);
        }

        private async Task CargarListasAsync(int? destinoId = null, int? vueloId = null, int? avionId = null)
        {
            var vuelos = await _context.Vuelos
                .OrderBy(v => v.FechaSalida)
                .Select(v => new
                {
                    v.Id,
                    Texto = v.Numero + " - " + v.Aerolinea + " (" + v.FechaSalida.ToString("dd/MM/yyyy") + ")"
                })
                .ToListAsync();

            ViewBag.Vuelos = new SelectList(vuelos, "Id", "Texto", vueloId);

            ViewBag.Destinos = new SelectList(
                await _context.Destinos.OrderBy(d => d.Nombre).ToListAsync(),
                "Id",
                "Nombre",
                destinoId);

            var aviones = await _context.Aviones
                .OrderBy(a => a.Modelo)
                .Select(a => new { a.Id, Texto = a.Modelo + " (" + a.Matricula + ")" })
                .ToListAsync();

            ViewBag.Aviones = new SelectList(aviones, "Id", "Texto", avionId);

            ViewBag.TiposEquipaje = await _context.TiposEquipaje.OrderBy(t => t.Id).ToListAsync();
        }
    }
}
