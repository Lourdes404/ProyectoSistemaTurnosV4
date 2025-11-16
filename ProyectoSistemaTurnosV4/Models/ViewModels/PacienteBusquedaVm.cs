

namespace ProyectoSistemaTurnosV4.Models.ViewModels
{
    
    public class PacienteBusquedaVm
    {
        public string? Dpi { get; set; }
        public string? Nombre { get; set; }   // nombre o nombre+apellido
        public string? Carne { get; set; }

        public List<Paciente> Resultados { get; set; } = new();
        public int? Total { get; set; }
        public bool Ejecutada { get; set; }   // para saber si ya se hizo una búsqueda
    }

}
