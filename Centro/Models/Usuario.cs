using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Centro.Models
{
    public class Usuario
    {
        [Key]
        public string Rut { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Rol { get; set; }
        public bool isBlock { get; set; }
    }
}