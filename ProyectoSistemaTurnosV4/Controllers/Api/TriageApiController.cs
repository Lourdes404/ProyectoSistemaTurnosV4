using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSistemaTurnosV4.Data;
using ProyectoSistemaTurnosV4.Models;

namespace ProyectoSistemaTurnosV4.Controllers.Api
{
    [ApiController]
    [Route("api/triage")]  // URL base: /api/triage
    public class TriageApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        public TriageApiController(AppDbContext db) => _db = db;

        // GET /api/triage
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lista = await _db.Triage
                .Include(t => t.Paciente)
                .Include(t => t.ClinicaDestino)
                .OrderByDescending(t => t.FechaRegistro)
                .Take(50)
                .ToListAsync();

            return Ok(lista);
        }

        // GET /api/triage/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var triage = await _db.Triage
                .Include(t => t.Paciente)
                .Include(t => t.ClinicaDestino)
                .FirstOrDefaultAsync(t => t.Id == id);

            return triage is null ? NotFound() : Ok(triage);
        }

        // POST /api/triage
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Triage model)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Si el paciente no existe, error
            if (!await _db.Pacientes.AnyAsync(p => p.Id == model.PacienteId))
                return BadRequest(new { message = "Paciente no encontrado" });

            // Si la clínica no existe, error
            if (!await _db.Clinicas.AnyAsync(c => c.Id == model.ClinicaDestinoId))
                return BadRequest(new { message = "Clínica no válida" });

            model.Id = 0;
            _db.Triage.Add(model);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        // PUT /api/triage/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] Triage model)
        {
            if (id != model.Id) return BadRequest("El ID no coincide");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            _db.Entry(model).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /api/triage/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var triage = await _db.Triage.FindAsync(id);
            if (triage is null) return NotFound();

            _db.Triage.Remove(triage);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
