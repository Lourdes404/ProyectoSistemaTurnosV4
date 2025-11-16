using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSistemaTurnosV4.Data;

[ApiController]
[Route("api/turnos")]

//[Authorize]
public class TurnosApiController : ControllerBase
{
    private readonly AppDbContext _db;
    public TurnosApiController(AppDbContext db) => _db = db;

    // GET /api/turnos/hoy?clinicaId=1
    [HttpGet("hoy")]
    public async Task<IActionResult> Hoy([FromQuery] int? clinicaId)
    {
        var hoy = DateTime.Today;
        var q = _db.Turnos
            .Include(t => t.Paciente)
            .Include(t => t.Clinica)
            .Where(t => t.CreadoEn >= hoy && t.CreadoEn < hoy.AddDays(1));

        if (clinicaId is not null) q = q.Where(t => t.ClinicaId == clinicaId);

        var data = await q.OrderBy(t => t.ClinicaId).ThenBy(t => t.Orden).ToListAsync();
        return Ok(data);
    }

    // GET /api/turnos/historial?desde=2025-11-01&hasta=2025-11-03
    [HttpGet("historial")]
    public async Task<IActionResult> Historial([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
    {
        var fin = hasta.Date.AddDays(1);
        var data = await _db.Turnos
            .Include(t => t.Paciente).Include(t => t.Clinica)
            .Where(t => t.CreadoEn >= desde.Date && t.CreadoEn < fin)
            .OrderByDescending(t => t.CreadoEn)
            .ToListAsync();
        return Ok(data);
    }

    // POST /api/turnos  (crear turno)
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] Turno model)
    {
        if (!await _db.Pacientes.AnyAsync(p => p.Id == model.PacienteId))
            return BadRequest(new { field = "PacienteId", message = "Paciente no encontrado" });
        if (!await _db.Clinicas.AnyAsync(c => c.Id == model.ClinicaId))
            return BadRequest(new { field = "ClinicaId", message = "Clínica no válida" });

        // ticket y orden del día por clínica
        var hoy = DateTime.Today;
        var maxOrden = await _db.Turnos
            .Where(t => t.ClinicaId == model.ClinicaId && t.CreadoEn >= hoy && t.CreadoEn < hoy.AddDays(1))
            .MaxAsync(t => (int?)t.Orden) ?? 0;

        model.Orden = maxOrden + 1;
        model.TicketPublico = $"{(await _db.Clinicas.FindAsync(model.ClinicaId))!.Nombre[..2].ToUpper()}-{model.Orden:000}";
        model.Id = 0; // identity
        model.Estado = TurnoEstado.EnEspera;
        model.CreadoEn = DateTime.Now;

        _db.Turnos.Add(model);
        _db.TurnoEventos.Add(new TurnoEvento { Turno = model, Estado = TurnoEstado.EnEspera });
        

        
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
    }

    // GET /api/turnos/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var t = await _db.Turnos.Include(x => x.Paciente).Include(x => x.Clinica)
                                .FirstOrDefaultAsync(x => x.Id == id);
        return t is null ? NotFound() : Ok(t);
    }

    // PUT /api/turnos/5/llamar
    [HttpPut("{id:int}/llamar")]
    public async Task<IActionResult> Llamar(int id)
    {
        var t = await _db.Turnos.FindAsync(id);
        if (t is null) return NotFound();
        t.Estado = TurnoEstado.Llamado; t.LlamadoEn = DateTime.Now;
        _db.TurnoEventos.Add(new TurnoEvento { TurnoId = id, Estado = TurnoEstado.Llamado });
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // PUT /api/turnos/5/finalizar
    [HttpPut("{id:int}/finalizar")]
    public async Task<IActionResult> Finalizar(int id)
    {
        var t = await _db.Turnos.FindAsync(id);
        if (t is null) return NotFound();
        t.Estado = TurnoEstado.Atendido; t.AtendidoEn = DateTime.Now;
        _db.TurnoEventos.Add(new TurnoEvento { TurnoId = id, Estado = TurnoEstado.Atendido });
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // PUT /api/turnos/5/ausente
    [HttpPut("{id:int}/ausente")]
    public async Task<IActionResult> Ausente(int id)
    {
        var t = await _db.Turnos.FindAsync(id);
        if (t is null) return NotFound();
        t.Estado = TurnoEstado.Ausente;
        _db.TurnoEventos.Add(new TurnoEvento { TurnoId = id, Estado = TurnoEstado.Ausente });
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
