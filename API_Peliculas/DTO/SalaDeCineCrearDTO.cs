using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API_Peliculas.DTO
{
    public class SalaDeCineCrearDTO
    {
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
    }
}
