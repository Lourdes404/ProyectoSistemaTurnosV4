using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSistemaTurnosV4.Data;

[Route("Turnos")]

//[Authorize(Roles = "Admin,Recepcion")]
//[Authorize]
public class TurnosController : Controller{
    private readonly AppDbContext _db;
    public TurnosController(AppDbContext db) => _db = db;

    // GET /Turnos
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var hoy = DateTime.Today;
        var turnosHoy = await _db.Turnos
            .Include(t => t.Paciente).Include(t => t.Clinica)
            .Where(t => t.CreadoEn >= hoy && t.CreadoEn < hoy.AddDays(1))
            .OrderBy(t => t.ClinicaId).ThenBy(t => t.Orden)
            .ToListAsync();

        ViewBag.Clinicas = await _db.Clinicas.OrderBy(c => c.Nombre).ToListAsync();
        return View(turnosHoy);
    }

    // GET /Turnos/Historial?fecha=2025-11-02
    [HttpGet("Historial")]
    public async Task<IActionResult> Historial(DateTime? fecha)
    {
        var dia = (fecha ?? DateTime.Today.AddDays(-1)).Date;
        var fin = dia.AddDays(1);
        var data = await _db.Turnos
            .Include(t => t.Paciente).Include(t => t.Clinica)
            .Where(t => t.CreadoEn >= dia && t.CreadoEn < fin)
            .OrderBy(t => t.ClinicaId).ThenBy(t => t.Orden)
            .ToListAsync();

        ViewBag.Fecha = dia;
        return View(data);
    }

    // GET: /Turnos/Crear
    //[HttpGet]
    //public async Task<IActionResult> Crear()
    //{
    //    // aquí llenas combos de pacientes / clínicas si hace falta
    //    return View();
    //}

    //// POST: /Turnos/Crear
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Crear(Turno model)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        // repoblar combos si tienes SelectList
    //        return View(model);
    //    }

    //    // 👇 copiamos la lógica que tienes en el API
    //    var hoy = DateTime.Today;

    //    var maxOrden = await _db.Turnos
    //        .Where(t => t.ClinicaId == model.ClinicaId
    //                    && t.CreadoEn >= hoy
    //                    && t.CreadoEn < hoy.AddDays(1))
    //        .MaxAsync(t => (int?)t.Orden) ?? 0;

    //    model.Orden = maxOrden + 1;
    //    model.TicketPublico = $"{(await _db.Clinicas.FindAsync(model.ClinicaId))!.Nombre[..3].ToUpper()}{model.Orden:D3}";
    //    model.Id = 0; // por si acaso, que lo genere la identity
    //    model.Estado = TurnoEstado.EnEspera;
    //    model.CreadoEn = DateTime.Now;

    //    _db.Turnos.Add(model);
    //    _db.TurnoEventos.Add(new TurnoEvento
    //    {
    //        Turno = model,
    //        Estado = TurnoEstado.EnEspera
    //    });

    //    await _db.SaveChangesAsync();

    //    return RedirectToAction(nameof(Index));
    //}
}

