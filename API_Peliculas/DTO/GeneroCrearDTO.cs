using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API_Peliculas.DTO
{
    public class GeneroCrearDTO
    {
        [Required]
        [MaxLength(40)]
        public string Nombre { get; set; }
    }
}
