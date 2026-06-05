using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasajesAeropuerto.Entities
{
    public class TipoEquipaje
    {
        public const int IdMano = 1;
        public const int IdValija = 2;
        public const int IdAdicional = 3;

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Recargo { get; set; }

        public ICollection<Equipaje> Equipajes { get; set; } = new List<Equipaje>();
    }
}
