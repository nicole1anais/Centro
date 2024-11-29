using Centro.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Profesionales> Profesionales { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Citas> Citas { get; set; }
    public DbSet<Pagos> Pagos { get; set; }
    public DbSet<Especialidad> Especialidad { get; set; }
    public DbSet<Contacto> Contactos { get; set; }
}