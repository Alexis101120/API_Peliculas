﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API_Peliculas.Entidades
{
    public class SalaDeCine : Iid
    {
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }

        public List<PeliculasSalasDeCine> PeliculasSalasDeCines { get; set; }
    }
}
