using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Requerido para [Column]

namespace PasajesAeropuerto.Entities
{
    public class Reserva
    {
        [Key]
        public int Id { get; set; }

        //[Required]
        //[MaxLength(20)]
        //public string CodigoReserva { get; set; } = string.Empty;

        public int CantPersonas { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCalculado { get; set; }

        [Required]
        public DateTime FechaEmision { get; set; }

        public decimal CalcularTotal()
        {
            return 0;
        }
    }
}