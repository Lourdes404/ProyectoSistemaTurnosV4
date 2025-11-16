using ProyectoSistemaTurnosV4.Models;
using System.ComponentModel.DataAnnotations;

public enum TurnoEstado : byte
{
    EnEspera = 0,   // recién creado
    Llamado = 1,    // médico lo llamó
    Atendido = 2,   // consulta finalizada
    Ausente = 3     // no se presentó
}


public class Turno
{
    public int Id { get; set; }

    [Required] public int PacienteId { get; set; }
    [Required] public int ClinicaId { get; set; }

    [Required] public TurnoEstado Estado { get; set; } = TurnoEstado.EnEspera;

    // Para ordenar la cola del día por clínica
    public int Orden { get; set; }

    // Ticket visible en pantalla (ej: MI-024)
    [MaxLength(20)] public string TicketPublico { get; set; } = "";

    // Fecha/hora de creación (para “hoy” vs “historial”)
    public DateTime CreadoEn { get; set; } = DateTime.Now;

    // Marcas opcionales
    public DateTime? LlamadoEn { get; set; }
    public DateTime? AtendidoEn { get; set; }

    // Navs (no validar en binding/api)
    [Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public Paciente? Paciente { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public Clinica? Clinica { get; set; }
}

