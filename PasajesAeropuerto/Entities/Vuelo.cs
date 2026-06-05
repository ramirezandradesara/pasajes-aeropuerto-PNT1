using System;
using System.ComponentModel.DataAnnotations;

namespace PasajesAeropuerto.Entities
{
    public class Vuelo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Numero { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Aerolinea { get; set; } = string.Empty;

        [Required]
        public DateTime FechaSalida { get; set; }

        [Required]
        public TimeSpan HoraSalida { get; set; }

        [Required]
        public TimeSpan HoraLlegada { get; set; }

        public bool EsTemporadaAlta { get; set; }

        public bool EsFinDeSemana()
        {
            return FechaSalida.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        }
    }
}