using System.ComponentModel.DataAnnotations;

namespace PasajesAeropuerto.Entities
{
    public class Pasajero
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Dni { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        public int? ReservaId { get; set; }

        public Reserva? Reserva { get; set; }

        public ICollection<Equipaje> Equipajes { get; set; } = new List<Equipaje>();
    }
}