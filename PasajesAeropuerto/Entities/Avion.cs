using System.ComponentModel.DataAnnotations;

namespace PasajesAeropuerto.Entities
{
    public class Avion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Modelo { get; set; } = string.Empty;

        public int Capacidad { get; set; }

        [Required]
        [MaxLength(20)]
        public string Matricula { get; set; } = string.Empty;
    }
}