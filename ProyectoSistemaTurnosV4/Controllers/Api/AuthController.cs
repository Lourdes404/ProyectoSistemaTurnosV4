using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSistemaTurnosV4.Data;
using ProyectoSistemaTurnosV4.Models;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokens;
    public AuthController(AppDbContext db, ITokenService tokens) { _db = db; _tokens = tokens; }

    public record LoginDto(string Email, string Password);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user is null) return Unauthorized(new { message = "Credenciales inválidas" });

        var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!ok) return Unauthorized(new { message = "Credenciales inválidas" });

        var token = _tokens.GenerarToken(user);
        return Ok(new
        {
            access_token = token,
            token_type = "Bearer",
            expires_in = 60 * 60 * 2, // 2h
            user = new { user.Id, user.Nombre, user.Email, user.Rol }
        });
    }
}

