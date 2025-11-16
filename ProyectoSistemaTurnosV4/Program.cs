using Microsoft.AspNetCore.Authentication.JwtBearer;     // <- si usas JwtBearerDefaults
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProyectoSistemaTurnosV4.Data;
using System.Text;

using ProyectoSistemaTurnosV4.Models;  




var builder = WebApplication.CreateBuilder(args);

// 1) Servicios (siempre ANTES de Build)
builder.Services.AddControllersWithViews();
// Registro del HttpClientFactory
builder.Services.AddHttpClient();

// DbContext (si aplica)
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

// Opción A: usando JwtBearerDefaults (requiere paquete + using arriba)
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

// Opción B (alternativa): usando literal "Bearer" (sin JwtBearerDefaults)
// builder.Services
//     .AddAuthentication("Bearer")
//     .AddJwtBearer(opt =>
//     {
//         opt.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = jwtSection["Issuer"],
//             ValidAudience = jwtSection["Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(key),
//             ClockSkew = TimeSpan.Zero
//         };
//     });

//builder.Services.AddAuthorization();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// 2) Build
var app = builder.Build();

// 3) Pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();   // <- SIEMPRE antes de Authorization
app.UseAuthorization();

// Rutas (solo una vez)
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
