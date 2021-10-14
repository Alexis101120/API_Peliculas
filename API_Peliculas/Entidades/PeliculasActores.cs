using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API_Peliculas.Entidades
{
    public class PeliculasActores
    {
        public int ActorId { get; set; }
        public int PeliculaId { get; set;}
        public string Personaje { get; set; }
        public int Orden { get; set;}

        [ForeignKey("ActorId")]
        public Actor Actor { get; set; }

        [ForeignKey("PeliculaId")]
        public Pelicula Pelicula { get; set; }

    }
}
