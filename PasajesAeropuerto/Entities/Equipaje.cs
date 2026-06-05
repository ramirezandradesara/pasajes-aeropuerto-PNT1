using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasajesAeropuerto.Entities
{
    public class Equipaje
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PasajeroId { get; set; }

        [ForeignKey(nameof(PasajeroId))]
        public Pasajero Pasajero { get; set; } = null!;

        [Required]
        public int TipoEquipajeId { get; set; }

        [ForeignKey(nameof(TipoEquipajeId))]
        public TipoEquipaje TipoEquipaje { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Cantidad { get; set; }
    }
}
