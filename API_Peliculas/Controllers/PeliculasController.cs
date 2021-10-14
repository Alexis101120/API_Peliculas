using API_Peliculas.DTO;
using API_Peliculas.Entidades;
using API_Peliculas.Helpers;
using API_Peliculas.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace API_Peliculas.Controllers
{
    [Route("api/peliculas")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private IAlmacenadorArchivos _almacenador;
        private string _contenedor = "Peliculas";


        public PeliculasController(ApplicationDbContext db, IMapper mapper, IAlmacenadorArchivos almacenador)
        {
            _db = db;
            _mapper = mapper;
            _almacenador = almacenador;
        }

        [HttpGet]
        public async Task<ActionResult<PeliculasIndexDTO>> Get()
        {
            var top = 5;
            var hoy = DateTime.Now;

            var proximosEstrenos = await _db.Peliculas.Where(x => x.FechaEstreno > hoy)
                .OrderBy(x => x.FechaEstreno)
                .Take(top)
                .ToListAsync();

            var enCines = await _db.Peliculas.Where(x => x.EnCines)
                .Take(top)
                .ToListAsync();

            var Resultado = new PeliculasIndexDTO();
            Resultado.FuturosEstrenos = _mapper.Map<List<PeliculaDTO>>(proximosEstrenos);
            Resultado.EnCines = _mapper.Map<List<PeliculaDTO>>(enCines);
            return Resultado;
        }

        [HttpGet("filtro")]
        public async Task<ActionResult<List<PeliculaDTO>>> filtro([FromQuery] FiltroPeliculasDTO filtroPeliculasDTO)
        {
            var query = _db.Peliculas.AsQueryable();
            if (!string.IsNullOrEmpty(filtroPeliculasDTO.Titulo))
            {
                query = query.Where(x => x.Titulo.Contains(filtroPeliculasDTO.Titulo));
            }

            if (filtroPeliculasDTO.EnCines)
            {
                query = query.Where(x => x.EnCines);
            }

            if (filtroPeliculasDTO.ProximosEstrenos)
            {
                var hoy = DateTime.Now;
                query = query.Where(x => x.FechaEstreno > hoy);
            }

            if(filtroPeliculasDTO.GeneroId != 0)
            {
                query = query.Where(x => x.PeliculasGeneros.Select(y => y.GeneroId).Contains(filtroPeliculasDTO.GeneroId));
            }

            if (!string.IsNullOrEmpty(filtroPeliculasDTO.CampoOrdenar))
            {
                var tipoOrden = filtroPeliculasDTO.OrdenAscendente ? "ascending" : "descending";
                query = query.OrderBy($"{filtroPeliculasDTO.CampoOrdenar} {tipoOrden}");
            }

            await HttpContext.InsertarParametrosPaginacion(query, filtroPeliculasDTO.CantidadRegistrosPorPagina);
            var peliculas = await query.Paginar(filtroPeliculasDTO.Paginacion).ToListAsync();
            return _mapper.Map<List<PeliculaDTO>>(peliculas);
        }

        [HttpGet("{id:int}", Name = "obtenerPelicula")]
        public async Task<ActionResult<PeliculaDetalleDTO>> Get(int id)
        {
            var Pelicula = await _db.Peliculas
                .Include(x => x.PeliculasActores).ThenInclude(x => x.Actor)
                .Include(x => x.PeliculasGeneros).ThenInclude(x => x.Genero)
                .FirstOrDefaultAsync(x => x.Id == id);
            Pelicula.PeliculasActores = Pelicula.PeliculasActores.OrderBy(x => x.Orden).ToList();

            return _mapper.Map<PeliculaDetalleDTO>(Pelicula);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculaCrearDTO item)
        {
            var PeliculaObj = _mapper.Map<Pelicula>(item);
            if (item.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await item.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(item.Poster.FileName);
                    PeliculaObj.Poster = await _almacenador.GuardarArchivo(contenido, extension, _contenedor, item.Poster.ContentType);
                }
            }
            AsignarOrdenActores(PeliculaObj);
            _db.Peliculas.Add(PeliculaObj);
            await _db.SaveChangesAsync();
            var peliculaDTO = _mapper.Map<PeliculaDTO>(PeliculaObj);
            return new CreatedAtRouteResult("obtenerPelicula", new { id = PeliculaObj.Id }, peliculaDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaCrearDTO item)
        {
            var PeliculaDb = await _db.Peliculas
                .Include(x=> x.PeliculasActores)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (PeliculaDb == null)
            {
                return NotFound();
            }
            PeliculaDb = _mapper.Map(item, PeliculaDb);
            if (item.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await item.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(item.Poster.FileName);
                    PeliculaDb.Poster = await _almacenador.EditarArchivo(contenido, extension, _contenedor, PeliculaDb.Poster, item.Poster.ContentType);
                }
            }
            AsignarOrdenActores(PeliculaDb);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument item)
        {
            if (item == null)
            {
                return BadRequest();
            }
            var PeliculaDb = await _db.Peliculas.FirstOrDefaultAsync(x => x.Id == id);
            if (PeliculaDb == null)
            {
                return BadRequest();
            }

            var PeliculaDTO = _mapper.Map<PeliculaPatchDTO>(PeliculaDb);
            item.ApplyTo(PeliculaDTO, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);
            var esValido = TryValidateModel(PeliculaDTO);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(PeliculaDTO, PeliculaDb);
            await _db.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var Existe = await _db.Peliculas.AnyAsync(x => x.Id == id);
            if (!Existe)
            {
                return NotFound();
            }
            _db.Remove(new Pelicula() { Id = id });
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private void AsignarOrdenActores(Pelicula pelicula)
        {
            if (pelicula.PeliculasActores != null)
            {
                for(int i=0; i< pelicula.PeliculasActores.Count; i++)
                {
                    pelicula.PeliculasActores[i].Orden = i;
                }
            }
        }
    }
}
