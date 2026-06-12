namespace PasajesAeropuerto.Models
{
    public class ReservaListadoViewModel
    {
        public int Id { get; set; }
        public string OrigenNombre { get; set; } = string.Empty;
        public string DestinoNombre { get; set; } = string.Empty;
        public string VueloNumero { get; set; } = string.Empty;
        public string Aerolinea { get; set; } = string.Empty;
        public DateTime FechaVuelo { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public string Clase { get; set; } = string.Empty;
        public decimal TotalCalculado { get; set; }
        public DateTime FechaEmision { get; set; }
        public List<PasajeroReservaViewModel> Pasajeros { get; set; } = new();
    }

    public class PasajeroReservaViewModel
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Equipajes { get; set; } = new();
    }
}
