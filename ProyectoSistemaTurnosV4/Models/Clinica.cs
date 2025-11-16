using System.ComponentModel.DataAnnotations;

namespace ProyectoSistemaTurnosV4.Models
{
    public class Clinica
    {
        public int Id { get; set; }

        [Required, MaxLength(80)]
        public string Nombre { get; set; } = null!;

        public bool Activa { get; set; } = true;
    }
}
