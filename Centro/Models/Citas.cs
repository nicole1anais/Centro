using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Centro.Models
{
    public class Citas
    {

        [Key]
        public int Id { get; set; }
        public bool Disponible { get; set; } = true;
        public string? Rut { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public int? Precio { get; set; }
        public int? EspecialidadId { get; set; }
        public Especialidad Especialidad { get; set; }
        public int? ProfesionalesId { get; set; }
        public virtual Profesionales Profesionales { get; set; }
    }
}