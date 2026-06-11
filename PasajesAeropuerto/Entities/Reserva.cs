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

        public int OrigenId { get; set; }

        public Origen Origen { get; set; } = null!;

        public int DestinoId { get; set; }

        public Destino Destino { get; set; } = null!;

        public int VueloId { get; set; }

        public Vuelo Vuelo { get; set; } = null!;

        public int PasajeroId { get; set; }

        public Pasajero Pasajero { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Clase { get; set; } = string.Empty;

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