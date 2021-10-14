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

namespace API_Peliculas.Controllers
{
    [Route("api/actores")]
    [ApiController]
    public class ActoresController : CustomBaseController
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private IAlmacenadorArchivos _almacenador;
        private const string Carpeta = "Actores";

        public ActoresController(ApplicationDbContext db, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos) : base(db,mapper)
        {
            _db = db;
            _mapper = mapper;
            _almacenador = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] Paginacion Paginacin)
        {
            return await Get<Actor, ActorDTO>(Paginacin);
        }

        [HttpGet("{id}", Name = "obtenerActor")]
        public async Task<ActionResult<ActorDTO>>Get(int id)
        {
            var Actor = await _db.Actores.FirstOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<ActorDTO>(Actor);
        }

        [HttpPost]
        public async Task<ActionResult<ActorCrearDTO>>Post([FromForm] ActorCrearDTO item)
        {
            var Actor = _mapper.Map<Actor>(item);
            if (item.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await item.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(item.Foto.FileName);
                    Actor.Foto = await _almacenador.GuardarArchivo(contenido, extension, Carpeta, item.Foto.ContentType);
                }
            }
            _db.Actores.Add(Actor);
            await _db.SaveChangesAsync();
            var Dto = _mapper.Map<ActorDTO>(Actor);
            return new CreatedAtRouteResult("obtenerActor", new { id = Actor.Id }, Dto);
        }

        [HttpPut("{id}")]
        //Cuando recibes algun archivo es fromform, cuando solo recibiras algun objeto ya sea json o xml es frombody
        public async Task<ActionResult<ActorCrearDTO>> Put(int id, [FromForm] ActorCrearDTO item)
        {
            var ActorDb = await _db.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if(ActorDb == null)
            {
                return NotFound();
            }

            ActorDb = _mapper.Map(item, ActorDb);

            if (item.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await item.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(item.Foto.FileName);
                    ActorDb.Foto = await _almacenador.EditarArchivo(contenido, extension, Carpeta,ActorDb.Foto, item.Foto.ContentType);
                }
            }
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> item)
        {
            if (item == null)
            {
                return BadRequest();
            }
            var ActorDb = await _db.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if(ActorDb == null)
            {
                return BadRequest();
            }

            var ActorDTO = _mapper.Map<ActorPatchDTO>(ActorDb);
            item.ApplyTo(ActorDTO,ModelState);
            var esValido = TryValidateModel(ActorDTO);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(ActorDTO, ActorDb);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var Existe = await _db.Actores.AnyAsync(x => x.Id == id);
            if (!Existe)
            {
                return NotFound();
            }

            _db.Actores.Remove(new Actor { Id = id });
            _db.SaveChanges();
            return NoContent();
        }
    }
}
