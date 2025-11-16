using Microsoft.EntityFrameworkCore;
using ProyectoSistemaTurnosV4.Models;

namespace ProyectoSistemaTurnosV4.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Clinica> Clinicas => Set<Clinica>();

        public DbSet<Paciente> Pacientes => Set<Paciente>();

        public DbSet<Triage> Triage => Set<Triage>();

        public DbSet<Turno> Turnos { get; set; }


        public DbSet<User> Users { get; set; }
        public DbSet<TurnoEvento> TurnoEventos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // seed mínimo para ver algo en la vista
            modelBuilder.Entity<Clinica>().HasData(
                new Clinica { Id = 1, Nombre = "Medicina Interna", Activa = true },
                new Clinica { Id = 2, Nombre = "Pediatría", Activa = true }
            );

            modelBuilder.Entity<Paciente>().HasData(
                new Paciente { Id = 1, Nombres = "Ana María", Apellidos = "Lopez", Documento = "1234567890101", Activo = true },
                new Paciente { Id = 2, Nombres = "Carlos", Apellidos = "Pérez", Documento = "9876543210101", Activo = true }
            );

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Triage>()
             .HasOne(t => t.Paciente)
             .WithMany(p => p.Triage)       // ya tenías ICollection<Triage> en Paciente
             .HasForeignKey(t => t.PacienteId)
             .OnDelete(DeleteBehavior.Restrict);   // evita cascadas raras

            modelBuilder.Entity<Triage>()
             .HasOne(t => t.ClinicaDestino)
             .WithMany()
             .HasForeignKey(t => t.ClinicaDestinoId)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Turno>()
                .HasIndex(t => new { t.ClinicaId, t.Estado, t.Orden });

            modelBuilder.Entity<TurnoEvento>()
              .HasOne(e => e.Turno)
              .WithMany() // o .WithMany("Eventos") si agregas colección en Turno
              .HasForeignKey(e => e.TurnoId)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
