using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSistemaTurnosV4.Data;
using ProyectoSistemaTurnosV4.Models;
using ProyectoSistemaTurnosV4.Models.ViewModels;
using ProyectoSistemaTurnosV4.Data;
using ProyectoSistemaTurnosV4.Models;
using ProyectoSistemaTurnosV4.Models.ViewModels;

//namespace ProyectoSistemaTurnosV4.Controllers
public class PacientesController : Controller
{
    private readonly AppDbContext _db;
    public PacientesController(AppDbContext db) => _db = db;

    // /Pacientes/Buscar?dpi=...&nombre=...&carne=...
    [HttpGet]
    public IActionResult Buscar() => View();
    public async Task<IActionResult> Buscar([FromQuery] PacienteBusquedaVm vm)
    {
        vm.Ejecutada = !string.IsNullOrWhiteSpace(vm.Dpi)
                    || !string.IsNullOrWhiteSpace(vm.Nombre)
                    || !string.IsNullOrWhiteSpace(vm.Carne);

        if (!vm.Ejecutada)
            return View(vm); // muestra solo el formulario

        IQueryable<Paciente> q = _db.Pacientes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(vm.Dpi))
            q = q.Where(p => p.Documento == vm.Dpi.Trim());

        if (!string.IsNullOrWhiteSpace(vm.Carne))
            q = q.Where(p => p.Carne == vm.Carne.Trim());

        if (!string.IsNullOrWhiteSpace(vm.Nombre))
        {
            var nombre = vm.Nombre.Trim();
            q = q.Where(p =>
                (p.Nombres + " " + p.Apellidos).Contains(nombre)); // LIKE simple
        }

        vm.Resultados = await q
            .OrderBy(p => p.Apellidos).ThenBy(p => p.Nombres)
            .Take(50)
            .ToListAsync();

        vm.Total = vm.Resultados.Count;
        return View(vm);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Paciente()); // formulario vacío
    }

    // POST: /Pacientes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombres,Apellidos,Documento,Carne,Telefono,Email,FechaNacimiento,Direccion,Activo")] Paciente model)
    {
        // Validación de servidor
        if (!string.IsNullOrWhiteSpace(model.Documento))
        {
            bool existeDpi = await _db.Pacientes.AnyAsync(p => p.Documento == model.Documento);
            if (existeDpi)
                ModelState.AddModelError(nameof(model.Documento), "Ya existe un paciente con este DPI.");
        }

        if (!ModelState.IsValid) return View(model);

        _db.Pacientes.Add(model);
        await _db.SaveChangesAsync();

        TempData["ok"] = "Paciente creado correctamente.";

        // Redirige a Buscar y precarga el DPI para que lo veas en la tabla
        return RedirectToAction("Buscar", new { dpi = model.Documento });
    }
}
