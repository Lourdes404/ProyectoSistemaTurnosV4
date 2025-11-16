using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSistemaTurnosV4.Data;
using ProyectoSistemaTurnosV4.Models;
using ProyectoSistemaTurnosV4.Models.ViewModels;

namespace ProyectoSistemaTurnosV4.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UsuariosController(AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: /Usuarios/Crear
        [HttpGet]
        public IActionResult Crear()
        {
            return View(new UsuarioCreateVm());
        }

        // POST: /Usuarios/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(UsuarioCreateVm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Validar que el correo no esté repetido
            var existeEmail = await _context.Users
                .AnyAsync(u => u.Email == model.Email);

            if (existeEmail)
            {
                ModelState.AddModelError(nameof(model.Email),
                    "Ya existe un usuario con ese correo.");
                return View(model);
            }

            // Mapear al modelo User de la BD
            // Mapear al modelo User
            var user = new User
            {
                Email = model.Email,
                Nombre = model.Nombre,
                Rol = model.Rol
            };

            // Hashear la contraseña ANTES de guardar
            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);



            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Credenciales incorrectas");
            }

            // Después de crear, redirigimos (puedes cambiarlo a donde quieras)
            return RedirectToAction("Index", "Home");
        }
    }
}
