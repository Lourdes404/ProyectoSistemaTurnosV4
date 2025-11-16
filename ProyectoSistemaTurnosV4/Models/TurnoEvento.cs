using ProyectoSistemaTurnosV4.Models;
using System.ComponentModel.DataAnnotations;

public class TurnoEvento
{
    public int Id { get; set; }
    public int TurnoId { get; set; }
    public TurnoEstado Estado { get; set; }
    public DateTime OcurrioEn { get; set; } = DateTime.Now;
    [MaxLength(120)] public string? Nota { get; set; }

    [Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public Turno? Turno { get; set; }
}
