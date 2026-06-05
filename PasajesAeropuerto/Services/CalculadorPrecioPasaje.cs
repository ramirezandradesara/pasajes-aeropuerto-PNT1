using PasajesAeropuerto.Entities;

namespace PasajesAeropuerto.Services
{
    public static class CalculadorPrecioPasaje
    {
        public const decimal FactorTemporadaAlta = 1.5m;
        public const decimal RecargoFinDeSemana = 0.15m;

        public static SimulacionPrecioResultado Calcular(
            Destino destino,
            Vuelo vuelo,
            string clase,
            int cantPersonas,
            bool equipajeValija,
            bool equipajeAdicional,
            IReadOnlyList<TipoEquipaje> tiposEquipaje)
        {
            var recargos = tiposEquipaje.ToDictionary(t => t.Id, t => t.Recargo);

            var factorClase = ObtenerFactorClase(clase);
            var kgIncluidos = ObtenerKgIncluidos(clase);

            var precioBase = destino.PrecioBase;
            var precioConClase = precioBase * factorClase;

            var precioTrasTemporada = precioConClase;
            var aplicoTemporadaAlta = vuelo.EsTemporadaAlta;
            if (aplicoTemporadaAlta)
            {
                precioTrasTemporada *= FactorTemporadaAlta;
            }

            var recargoFinDeSemana = 0m;
            var esFinDeSemana = vuelo.EsFinDeSemana();
            var precioTrasFinDeSemana = precioTrasTemporada;
            if (esFinDeSemana)
            {
                recargoFinDeSemana = precioTrasTemporada * RecargoFinDeSemana;
                precioTrasFinDeSemana += recargoFinDeSemana;
            }

            var recargoValija = equipajeValija && recargos.TryGetValue(TipoEquipaje.IdValija, out var rv)
                ? rv
                : 0m;
            var recargoAdicional = equipajeAdicional && recargos.TryGetValue(TipoEquipaje.IdAdicional, out var ra)
                ? ra
                : 0m;
            var precioPorPersona = precioTrasFinDeSemana + recargoValija + recargoAdicional;
            var total = precioPorPersona * cantPersonas;

            return new SimulacionPrecioResultado
            {
                DestinoNombre = destino.Nombre,
                VueloNumero = vuelo.Numero,
                FechaVuelo = vuelo.FechaSalida,
                Clase = clase,
                KgIncluidos = kgIncluidos,
                CantPersonas = cantPersonas,
                PrecioBaseDestino = precioBase,
                FactorClase = factorClase,
                PrecioConClase = precioConClase,
                AplicoTemporadaAlta = aplicoTemporadaAlta,
                FactorTemporadaAlta = FactorTemporadaAlta,
                PrecioTrasTemporada = precioTrasTemporada,
                EsFinDeSemana = esFinDeSemana,
                RecargoFinDeSemana = recargoFinDeSemana,
                EquipajeValija = equipajeValija,
                RecargoValija = recargoValija,
                EquipajeAdicional = equipajeAdicional,
                RecargoEquipajeAdicional = recargoAdicional,
                PrecioPorPersona = precioPorPersona,
                Total = total
            };
        }

        public static decimal ObtenerFactorClase(string clase) => clase switch
        {
            "Primera" => 3.5m,
            "Business" => 2.0m,
            _ => 1.0m
        };

        public static int ObtenerKgIncluidos(string clase) => clase switch
        {
            "Primera" => 40,
            "Business" => 30,
            _ => 23
        };
    }

    public class SimulacionPrecioResultado
    {
        public string DestinoNombre { get; set; } = string.Empty;
        public string VueloNumero { get; set; } = string.Empty;
        public DateTime FechaVuelo { get; set; }
        public string Clase { get; set; } = string.Empty;
        public int KgIncluidos { get; set; }
        public int CantPersonas { get; set; }
        public decimal PrecioBaseDestino { get; set; }
        public decimal FactorClase { get; set; }
        public decimal PrecioConClase { get; set; }
        public bool AplicoTemporadaAlta { get; set; }
        public decimal FactorTemporadaAlta { get; set; }
        public decimal PrecioTrasTemporada { get; set; }
        public bool EsFinDeSemana { get; set; }
        public decimal RecargoFinDeSemana { get; set; }
        public bool EquipajeValija { get; set; }
        public decimal RecargoValija { get; set; }
        public bool EquipajeAdicional { get; set; }
        public decimal RecargoEquipajeAdicional { get; set; }
        public decimal PrecioPorPersona { get; set; }
        public decimal Total { get; set; }
    }
}
