using System.ComponentModel.DataAnnotations;

namespace PasajesAeropuerto.Entities
{
    public class Origen
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
    }
}
