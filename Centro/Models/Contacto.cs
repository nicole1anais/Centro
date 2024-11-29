using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Centro.Models
{
    public class Contacto
    {
        [Key]
        public int idContacto { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string Mensaje { get; set; }
    }
}