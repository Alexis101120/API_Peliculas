using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Peliculas.DTO
{
    public class Paginacion
    {
        public int Pagina { get; set; } = 1;
        private int CantidadRegistrosPagina = 10;
        private readonly int CantidadMaximaPorPagina = 50;
        
        public int cantidadRegistosPagina
        {
            get => CantidadRegistrosPagina;
            set
            {
                CantidadRegistrosPagina = (value > CantidadMaximaPorPagina) ? CantidadMaximaPorPagina : value;
            }
        }
    }
}
