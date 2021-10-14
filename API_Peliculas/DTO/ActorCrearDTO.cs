using API_Peliculas.Validaciones;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API_Peliculas.DTO
{
    public class ActorCrearDTO
    {
        [Required]
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
        [PesoImagenValidacion(4)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imegen)]
        public IFormFile Foto { get; set; }
    }
}
