using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API_Peliculas.Entidades
{
    public class PeliculasGeneros
    {
        public int GeneroId { get; set; }
        public int PeliculaId { get; set; }

        [ForeignKey("GeneroId")]
        public Genero Genero { get; set; }
        [ForeignKey("PeliculaId")]
        public Pelicula Pelicula { get; set; }
    }
}
