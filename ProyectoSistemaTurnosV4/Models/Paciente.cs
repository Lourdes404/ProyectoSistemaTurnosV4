using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProyectoSistemaTurnosV4.Models
{
    [Index(nameof(Documento))]
    [Index(nameof(Carne))]
    public class Paciente
    {
        public int Id { get; set; }

        [Required, MaxLength(80)]
        public string Nombres { get; set; } = null!;

        [Required, MaxLength(80)]
        public string Apellidos { get; set; } = null!;

        [MaxLength(25)]
        public string? Documento { get; set; } // DPI (único si querés)

        [MaxLength(25)]
        public string? Carne { get; set; } // No. de carné interno o expediente

        [MaxLength(20)]
        public string? Telefono { get; set; }

        [MaxLength(150), EmailAddress]
        public string? Email { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        [MaxLength(200)]
        public string? Direccion { get; set; }

        public bool Activo { get; set; } = true;

        // Relación con triage o consultas
        //public ICollection<Triage> Triage { get; set; } = new List<Triage>();

        public ICollection<Triage> Triage { get; set; } = new List<Triage>();


    }
}
