using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Centro.Models
{
    public class Profesionales
    {
        [Key]
        public int Id { get; set; }
        public string Rut { get; set; }
        public string Imagen { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public int EspecialidadId { get; set; }
        public Especialidad Especialidad { get; set; }
        public string SedeEgreso { get; set; }
        public string Diplomados { get; set; }

        // Campos nuevos para las coordenadas
        public double Latitud { get; set; }
        public double Longitud { get; set; }

    }
}