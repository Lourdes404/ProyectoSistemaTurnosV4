using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSistemaTurnosV4.Data;
using ProyectoSistemaTurnosV4.Models;

public class TriageController : Controller
{
    private readonly AppDbContext _db;
    public TriageController(AppDbContext db) => _db = db;

    // Si el paciente ya tiene triage reciente -> Edit; si no -> Create
    // GET: /Triage/Upsert?pacienteId=2
    [HttpGet]
    public async Task<IActionResult> Upsert(int pacienteId)
    {
        var paciente = await _db.Pacientes.FindAsync(pacienteId);
        if (paciente is null) return NotFound();

        // último triage del paciente (si existe)
        var triage = await _db.Triage
            .AsNoTracking()
            .OrderByDescending(t => t.FechaRegistro)
            .FirstOrDefaultAsync(t => t.PacienteId == pacienteId);

        ViewBag.Paciente = paciente;
        ViewBag.Clinicas = await _db.Clinicas
            .OrderBy(c => c.Nombre)
            .ToListAsync();

        return View(triage ?? new Triage { PacienteId = pacienteId });
    }

    // POST: /Triage/Upsert
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(Triage model)
    {
        // Re-cargar datos de la vista SIEMPRE que algo salga mal
        async Task LoadViewBags()
        {
            ViewBag.Paciente = await _db.Pacientes.FindAsync(model.PacienteId);
            ViewBag.Clinicas = await _db.Clinicas.OrderBy(c => c.Nombre).ToListAsync();
        }

        if (!ModelState.IsValid)
        {
            await LoadViewBags();
            return View(model);
        }

        // Validaciones de FK amistosas
        if (!await _db.Pacientes.AnyAsync(p => p.Id == model.PacienteId))
            ModelState.AddModelError("", "Paciente no encontrado.");
        if (!await _db.Clinicas.AnyAsync(c => c.Id == model.ClinicaDestinoId))
            ModelState.AddModelError(nameof(model.ClinicaDestinoId), "Seleccione una clínica válida.");

        if (!ModelState.IsValid)
        {
            await LoadViewBags();
            return View(model);
        }

        try
        {
            if (model.Id == 0)
                _db.Triage.Add(model);
            else
                _db.Entry(model).State = EntityState.Modified;

            await _db.SaveChangesAsync();
            TempData["ok"] = "Triage guardado correctamente.";
            return RedirectToAction("Buscar", "Pacientes", new { dpi = "" });
        }
        catch (DbUpdateException ex)
        {
            // Mostrar detalle de SQL/EF en la vista
            ModelState.AddModelError("", ex.InnerException?.Message ?? ex.Message);
            await LoadViewBags();
            return View(model);
        }
    }

    //[HttpPost("api/triage")]
    //public async Task<IActionResult> CrearTriageAsync([FromBody] Triage model)
    //{
        

    //    if (!ModelState.IsValid)
    //        return BadRequest(ModelState);

    //    if (!await _db.Pacientes.AnyAsync(p => p.Id == model.PacienteId))
    //        return BadRequest(new { field = "PacienteId", message = "Paciente no encontrado." });

    //    _db.Triage.Add(model);
    //    _db.SaveChanges();
    //    return Ok(model);
    //}

}
