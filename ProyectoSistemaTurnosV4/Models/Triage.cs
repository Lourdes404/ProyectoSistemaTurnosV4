using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProyectoSistemaTurnosV4.Models
{
    [Index(nameof(PacienteId), nameof(FechaRegistro))] // consultas por paciente y fecha
    public class Triage
    {
        public int Id { get; set; }

        [Required]
        public int PacienteId { get; set; }

        [Required]
        public int ClinicaDestinoId { get; set; }

        [Required, MaxLength(10)]                   // Alta | Media | Baja
        public string NivelPrioridad { get; set; } = "Media";

        [MaxLength(120)]
        public string? MotivoConsulta { get; set; }

        [MaxLength(400)]
        public string? Sintomas { get; set; }

        [MaxLength(200)]
        public string? SignosVitales { get; set; }  // ej: "TA 120/80, FC 72, Temp 37.2"

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Navs
        //public Paciente Paciente { get; set; } = null!;
        //public Clinica? ClinicaDestino { get; set; }

        // 👇 evita validación y binding de estas referencias
        [ValidateNever, JsonIgnore] public Paciente? Paciente { get; set; }
        [ValidateNever, JsonIgnore] public Clinica? ClinicaDestino { get; set; }
    }
}
