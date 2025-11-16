

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSistemaTurnosV4.Data;
using ProyectoSistemaTurnosV4.Models;

namespace ProyectoSistemaTurnosV4.Controllers.Api
{
    [ApiController]
    [Route("api/pacientes")]
    public class PacientesApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PacientesApiController(AppDbContext db) => _db = db;

        // GET api/pacientes?dpi=...&nombre=...&carne=...
        [HttpGet]
        public async Task<IActionResult> GetPacientes([FromQuery] string? dpi, [FromQuery] string? nombre, [FromQuery] string? carne) 
        {
            var query = _db.Pacientes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(dpi))
                query = query.Where(p => p.Documento == dpi);
            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(p => (p.Nombres + " " + p.Apellidos).Contains(nombre));
            if (!string.IsNullOrWhiteSpace(carne))
                query = query.Where(p => p.Carne == carne);

            return Ok(await query.Take(50).ToListAsync());
        }

        // GET /api/pacientes/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Paciente>> GetById(int id)
        {
            var paciente = await _db.Pacientes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            return paciente is null ? NotFound() : Ok(paciente);
        }


        // GET /api/pacientes/exists/2  -> true/false
        [HttpGet("api/pacientes/exists/{id:int}")]
        public async Task<bool> PacienteExists(int id) =>
            await _db.Pacientes.AnyAsync(p => p.Id == id);


        // POST /api/pacientes
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Paciente model)
        {
            // Validación por data annotations
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // DPI duplicado (si viene)
            if (!string.IsNullOrWhiteSpace(model.Documento))
            {
                var dup = await _db.Pacientes.AnyAsync(p => p.Documento == model.Documento);
                if (dup) return Conflict(new { field = "Documento", message = "Ya existe un paciente con este DPI." });
            }

            _db.Pacientes.Add(model);
            await _db.SaveChangesAsync();

            // Devuelve 201 con Location: /api/pacientes/{id}
            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

    }
}

