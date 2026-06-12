namespace PasajesAeropuerto.Models
{
    public class PasajeroFormModel
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EquipajeValija { get; set; }
        public bool EquipajeAdicional { get; set; }
    }
}
