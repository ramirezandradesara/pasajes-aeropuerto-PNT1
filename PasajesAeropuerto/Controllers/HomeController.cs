using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PasajesAeropuerto.Data;
using PasajesAeropuerto.Entities;
using PasajesAeropuerto.Models;
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

        [HttpGet]
        public async Task<IActionResult> MisReservas()
        {
            var reservas = await _context.Reservas
                .AsNoTracking()
                .Include(r => r.Origen)
                .Include(r => r.Destino)
                .Include(r => r.Vuelo)
                .Include(r => r.Pasajeros)
                    .ThenInclude(p => p.Equipajes)
                    .ThenInclude(e => e.TipoEquipaje)
                .OrderByDescending(r => r.FechaEmision)
                .ToListAsync();

            var model = reservas.Select(r => new ReservaListadoViewModel
            {
                Id = r.Id,
                OrigenNombre = r.Origen.Nombre,
                DestinoNombre = r.Destino.Nombre,
                VueloNumero = r.Vuelo.Numero,
                Aerolinea = r.Vuelo.Aerolinea,
                FechaVuelo = r.Vuelo.FechaSalida,
                HoraSalida = r.Vuelo.HoraSalida,
                Clase = r.Clase,
                TotalCalculado = r.TotalCalculado,
                FechaEmision = r.FechaEmision,
                Pasajeros = r.Pasajeros
                    .OrderBy(p => p.Id)
                    .Select(p => new PasajeroReservaViewModel
                    {
                        Nombre = p.Nombre,
                        Apellido = p.Apellido,
                        Dni = p.Dni,
                        Email = p.Email,
                        Equipajes = p.Equipajes
                            .OrderBy(e => e.TipoEquipajeId)
                            .Select(e => e.TipoEquipaje.Nombre)
                            .ToList()
                    })
                    .ToList()
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> VerPrecios(
            int OrigenId,
            int DestinoId,
            int VueloId,
            string Clase,
            int CantPersonas,
            DateTime? FechaDesde,
            DateTime? FechaHasta,
            List<PasajeroFormModel>? Pasajeros)
        {
            var errorFechas = ValidarFechasViaje(FechaDesde, FechaHasta);
            if (errorFechas is not null)
            {
                ViewBag.Error = errorFechas;
                await RestaurarFormularioAsync(OrigenId, DestinoId, VueloId, Clase, CantPersonas, FechaDesde, FechaHasta, Pasajeros);
                return View("Index");
            }

            var error = await ValidarDatosViajeAsync(OrigenId, DestinoId, VueloId, CantPersonas, FechaDesde, FechaHasta);
            if (error is not null)
            {
                ViewBag.Error = error;
                await RestaurarFormularioAsync(OrigenId, DestinoId, VueloId, Clase, CantPersonas, FechaDesde, FechaHasta, Pasajeros);
                return View("Index");
            }

            var destino = await _context.Destinos.FindAsync(DestinoId);
            var vuelo = await _context.Vuelos.FindAsync(VueloId);
            var clase = string.IsNullOrWhiteSpace(Clase) ? "Economica" : Clase;

            var tiposEquipaje = await ObtenerTiposEquipajeAsync();
            ViewBag.Simulacion = CalculadorPrecioPasaje.Calcular(
                destino!, vuelo!, clase, CantPersonas, false, false, tiposEquipaje);
            ViewBag.PreciosVistos = true;

            await RestaurarFormularioAsync(OrigenId, DestinoId, VueloId, clase, CantPersonas, FechaDesde, FechaHasta, null);
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AgregarPasaje(
            int OrigenId,
            int DestinoId,
            int VueloId,
            string Clase,
            int CantPersonas,
            DateTime? FechaDesde,
            DateTime? FechaHasta,
            List<PasajeroFormModel>? Pasajeros,
            bool PreciosVistos)
        {
            var pasajeros = NormalizarPasajeros(Pasajeros, CantPersonas);
            await RestaurarFormularioAsync(OrigenId, DestinoId, VueloId, Clase, CantPersonas, FechaDesde, FechaHasta, pasajeros);

            if (!PreciosVistos)
            {
                ViewBag.Error = "Primero debés hacer clic en «Ver precios».";
                return View("Index");
            }

            var errorFechas = ValidarFechasViaje(FechaDesde, FechaHasta);
            if (errorFechas is not null)
            {
                ViewBag.Error = errorFechas;
                await RecalcularYMostrarPreciosAsync(DestinoId, VueloId, Clase, CantPersonas);
                return View("Index");
            }

            var errorPasajeros = ValidarPasajeros(pasajeros, CantPersonas);
            if (errorPasajeros is not null)
            {
                ViewBag.Error = errorPasajeros;
                await RecalcularYMostrarPreciosAsync(DestinoId, VueloId, Clase, CantPersonas);
                return View("Index");
            }

            var errorViaje = await ValidarDatosViajeAsync(OrigenId, DestinoId, VueloId, CantPersonas, FechaDesde, FechaHasta);
            if (errorViaje is not null)
            {
                ViewBag.Error = errorViaje;
                return View("Index");
            }

            var destino = await _context.Destinos.FindAsync(DestinoId);
            var vuelo = await _context.Vuelos.FindAsync(VueloId);
            var clase = string.IsNullOrWhiteSpace(Clase) ? "Economica" : Clase;
            var tiposEquipaje = await ObtenerTiposEquipajeAsync();
            var total = CalculadorPrecioPasaje.CalcularTotalReserva(
                destino!,
                vuelo!,
                clase,
                pasajeros.Select(p => (p.EquipajeValija, p.EquipajeAdicional)),
                tiposEquipaje);

            var reserva = new Reserva
            {
                OrigenId = OrigenId,
                DestinoId = DestinoId,
                VueloId = VueloId,
                Clase = clase,
                FechaEmision = DateTime.Now,
                TotalCalculado = total
            };
            _context.Reservas.Add(reserva);

            foreach (var form in pasajeros)
            {
                var pasajero = new Pasajero
                {
                    Nombre = form.Nombre.Trim(),
                    Apellido = form.Apellido.Trim(),
                    Dni = form.Dni.Trim(),
                    Email = form.Email.Trim(),
                    Reserva = reserva
                };
                AgregarEquipaje(pasajero, form.EquipajeValija, form.EquipajeAdicional);
                _context.Pasajeros.Add(pasajero);
            }

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                $"Pasaje emitido. Reserva #{reserva.Id} — {pasajeros.Count} persona(s) — Total: {reserva.TotalCalculado:C}";
            return RedirectToAction(nameof(Index));
        }

        private static List<PasajeroFormModel> NormalizarPasajeros(List<PasajeroFormModel>? pasajeros, int cantPersonas)
        {
            var lista = pasajeros ?? new List<PasajeroFormModel>();
            while (lista.Count < cantPersonas)
            {
                lista.Add(new PasajeroFormModel());
            }

            if (lista.Count > cantPersonas)
            {
                lista = lista.Take(cantPersonas).ToList();
            }

            return lista;
        }

        private static string? ValidarPasajeros(List<PasajeroFormModel> pasajeros, int cantPersonas)
        {
            if (pasajeros.Count != cantPersonas)
            {
                return "La cantidad de pasajeros no coincide con la cantidad de personas.";
            }

            for (var i = 0; i < pasajeros.Count; i++)
            {
                var p = pasajeros[i];
                if (string.IsNullOrWhiteSpace(p.Nombre) ||
                    string.IsNullOrWhiteSpace(p.Apellido) ||
                    string.IsNullOrWhiteSpace(p.Dni) ||
                    string.IsNullOrWhiteSpace(p.Email))
                {
                    return $"Completá todos los datos del pasajero {i + 1}.";
                }
            }

            var dnis = pasajeros
                .Select(p => p.Dni.Trim())
                .Where(d => !string.IsNullOrEmpty(d))
                .ToList();
            if (dnis.Count != dnis.Distinct(StringComparer.OrdinalIgnoreCase).Count())
            {
                return "No puede haber dos pasajeros con el mismo DNI.";
            }

            return null;
        }

        private static void AgregarEquipaje(Pasajero pasajero, bool equipajeValija, bool equipajeAdicional)
        {
            pasajero.Equipajes.Add(new Equipaje
            {
                TipoEquipajeId = TipoEquipaje.IdMano,
                Cantidad = 1
            });
            if (equipajeValija)
            {
                pasajero.Equipajes.Add(new Equipaje
                {
                    TipoEquipajeId = TipoEquipaje.IdValija,
                    Cantidad = 1
                });
            }
            if (equipajeAdicional)
            {
                pasajero.Equipajes.Add(new Equipaje
                {
                    TipoEquipajeId = TipoEquipaje.IdAdicional,
                    Cantidad = 1
                });
            }
        }

        private static string? ValidarFechasViaje(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            if (fechaDesde.HasValue && fechaHasta.HasValue &&
                fechaDesde.Value.Date > fechaHasta.Value.Date)
            {
                return "La fecha «Desde» no puede ser posterior a «Hasta».";
            }

            return null;
        }

        private async Task<string?> ValidarDatosViajeAsync(
            int origenId,
            int destinoId,
            int vueloId,
            int cantPersonas,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            if (origenId <= 0 || destinoId <= 0 || vueloId <= 0)
            {
                return "Seleccioná origen, destino y vuelo.";
            }

            if (!fechaDesde.HasValue || !fechaHasta.HasValue)
            {
                return "Seleccioná el rango de fechas del viaje.";
            }

            if (cantPersonas < 1 || cantPersonas > 10)
            {
                return "La cantidad de personas debe ser entre 1 y 10.";
            }

            if (!await _context.Origenes.AnyAsync(o => o.Id == origenId))
            {
                return "Origen no válido.";
            }

            if (!await _context.Destinos.AnyAsync(d => d.Id == destinoId))
            {
                return "Destino no válido.";
            }

            if (!await _context.Vuelos.AnyAsync(v => v.Id == vueloId))
            {
                return "Vuelo no válido.";
            }

            var fechaSalida = await _context.Vuelos.AsNoTracking()
                .Where(v => v.Id == vueloId)
                .Select(v => (DateTime?)v.FechaSalida)
                .FirstOrDefaultAsync();

            if (fechaSalida is null)
            {
                return "Vuelo no válido.";
            }

            var fechaVuelo = fechaSalida.Value.Date;
            if (fechaVuelo < fechaDesde.Value.Date || fechaVuelo > fechaHasta.Value.Date)
            {
                return "El vuelo seleccionado no está disponible en el rango de fechas elegido.";
            }

            return null;
        }

        private async Task RecalcularYMostrarPreciosAsync(
            int destinoId, int vueloId, string clase, int cantPersonas)
        {
            var destino = await _context.Destinos.FindAsync(destinoId);
            var vuelo = await _context.Vuelos.FindAsync(vueloId);
            if (destino is null || vuelo is null) return;

            var tiposEquipaje = await ObtenerTiposEquipajeAsync();
            ViewBag.Simulacion = CalculadorPrecioPasaje.Calcular(
                destino, vuelo, string.IsNullOrWhiteSpace(clase) ? "Economica" : clase,
                cantPersonas, false, false, tiposEquipaje);
            ViewBag.PreciosVistos = true;
        }

        private async Task<List<TipoEquipaje>> ObtenerTiposEquipajeAsync()
        {
            return await _context.TiposEquipaje.OrderBy(t => t.Id).ToListAsync();
        }

        private async Task RestaurarFormularioAsync(
            int origenId,
            int destinoId,
            int vueloId,
            string? clase,
            int cantPersonas,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            List<PasajeroFormModel>? pasajeros)
        {
            ViewBag.Clase = string.IsNullOrWhiteSpace(clase) ? "Economica" : clase;
            ViewBag.CantPersonas = cantPersonas;
            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
            ViewBag.Pasajeros = NormalizarPasajeros(pasajeros, cantPersonas);
            await CargarListasAsync(
                origenId > 0 ? origenId : null,
                destinoId > 0 ? destinoId : null,
                vueloId > 0 ? vueloId : null,
                fechaDesde,
                fechaHasta);
        }

        private async Task CargarListasAsync(
            int? origenId = null,
            int? destinoId = null,
            int? vueloId = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null)
        {
            var origenes = await _context.Origenes.OrderBy(o => o.Nombre).ToListAsync();
            ViewBag.OrigenesLista = origenes;
            ViewBag.OrigenIdSeleccionado = origenId;
            ViewBag.Origenes = new SelectList(origenes, "Id", "Nombre", origenId);

            var destinos = await _context.Destinos.OrderBy(d => d.Nombre).ToListAsync();
            ViewBag.DestinosLista = destinos;
            ViewBag.DestinoIdSeleccionado = destinoId;
            ViewBag.Destinos = new SelectList(destinos, "Id", "Nombre", destinoId);

            var vuelosTodos = await _context.Vuelos
                .OrderBy(v => v.FechaSalida)
                .ThenBy(v => v.HoraSalida)
                .ToListAsync();

            ViewBag.VuelosTodosLista = vuelosTodos;

            var vuelos = vuelosTodos.AsEnumerable();
            if (fechaDesde.HasValue)
            {
                vuelos = vuelos.Where(v => v.FechaSalida.Date >= fechaDesde.Value.Date);
            }

            if (fechaHasta.HasValue)
            {
                vuelos = vuelos.Where(v => v.FechaSalida.Date <= fechaHasta.Value.Date);
            }

            var vuelosFiltrados = vuelos.ToList();

            ViewBag.VuelosLista = vuelosFiltrados;
            ViewBag.VueloIdSeleccionado = vueloId;
            ViewBag.Vuelos = new SelectList(
                vuelosFiltrados.Select(v => new
                {
                    v.Id,
                    Texto = v.Numero + " - " + v.Aerolinea + " (" + v.FechaSalida.ToString("dd/MM/yyyy") + ")"
                }),
                "Id",
                "Texto",
                vueloId);

            ViewBag.TiposEquipaje = await _context.TiposEquipaje.OrderBy(t => t.Id).ToListAsync();
        }
    }
}
