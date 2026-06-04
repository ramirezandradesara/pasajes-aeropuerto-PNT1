using Microsoft.EntityFrameworkCore;
using PasajesAeropuerto.Entities;
using System;

namespace PasajesAeropuerto.Data
{
    public class PasajesAeropuertoContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Data Source=localhost\SQLEXPRESS;
                  Initial Catalog=AeropuertoDB;
                  Integrated Security=True;
                  TrustServerCertificate=True");
        }

        public DbSet<Avion> Aviones { get; set; }
        public DbSet<Destino> Destinos { get; set; }
        public DbSet<Pasajero> Pasajeros { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Vuelo> Vuelos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Avion>().HasData(
                new Avion { Id = 1, Modelo = "Boeing 737", Capacidad = 160, Matricula = "LV-CKS" },
                new Avion { Id = 2, Modelo = "Airbus A320", Capacidad = 150, Matricula = "LV-GUR" },
                new Avion { Id = 3, Modelo = "Embraer 190", Capacidad = 96, Matricula = "LV-CHO" }
            );

            modelBuilder.Entity<Destino>().HasData(
                new Destino { Id = 1, Nombre = "Buenos Aires", Km = 0.0, PrecioBase = 0.00m },
                new Destino { Id = 2, Nombre = "Bariloche", Km = 1350.5, PrecioBase = 45000.00m },
                new Destino { Id = 3, Nombre = "Mendoza", Km = 1050.2, PrecioBase = 32000.00m },
                new Destino { Id = 4, Nombre = "Córdoba", Km = 700.0, PrecioBase = 22000.00m }
            );

            modelBuilder.Entity<Vuelo>().HasData(
                new Vuelo
                {
                    Id = 1,
                    Numero = "AR1400",
                    Aerolinea = "Aerolíneas Argentinas",
                    FechaSalida = new DateTime(2026, 6, 15),
                    HoraSalida = new TimeSpan(8, 30, 0),
                    HoraLlegada = new TimeSpan(10, 45, 0),
                    EsTemporadaAlta = false
                },
                new Vuelo
                {
                    Id = 2,
                    Numero = "WJ3420",
                    Aerolinea = "JetSMART",
                    FechaSalida = new DateTime(2026, 7, 20),
                    HoraSalida = new TimeSpan(14, 15, 0),
                    HoraLlegada = new TimeSpan(16, 30, 0),
                    EsTemporadaAlta = true
                },
                new Vuelo
                {
                    Id = 3,
                    Numero = "FB5200",
                    Aerolinea = "Flybondi",
                    FechaSalida = new DateTime(2026, 6, 21),
                    HoraSalida = new TimeSpan(19, 0, 0),
                    HoraLlegada = new TimeSpan(20, 30, 0),
                    EsTemporadaAlta = false
                }
            );
        }
    }
}