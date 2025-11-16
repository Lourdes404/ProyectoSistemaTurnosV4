public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Rol { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
}

