using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSistemaTurnosV4.Data;

public class ClinicasController : Controller
{
    private readonly AppDbContext _db;
    public ClinicasController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var clinicas = await _db.Clinicas.AsNoTracking().ToListAsync();
        return View(clinicas);
    }
}
