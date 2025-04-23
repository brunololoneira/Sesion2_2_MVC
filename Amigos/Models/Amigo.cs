using System.ComponentModel.DataAnnotations;

namespace Amigos.Models
{
    namespace TuProyecto.Models
    {
        public class Amigo
        {
            public int ID { get; set; }  // Clave primaria

            [Display(Name = "Nombre")]
            public string? Name { get; set; }  // Nombre del amigo

            [Display(Name = "Longitud")]
            public string? Longi { get; set; } // Longitud

            [Display(Name = "Latitud")]
            public string? Lati { get; set; }  // Latitud
        }
    }
}
