using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Centro.Models
{
    public class Pagos
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public int IdCitas { get; set; }
        public Citas Citas { get; set; }

    }
}