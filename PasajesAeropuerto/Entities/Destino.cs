using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasajesAeropuerto.Entities
{
    public class Destino
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public double Km { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioBase { get; set; }
    }
}