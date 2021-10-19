using API_Peliculas.DTO;
using API_Peliculas.Entidades;
using API_Peliculas.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Peliculas.Controllers
{
    [Route("api/generos")]
    [ApiController]
    public class GenerosController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GenerosController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO>>> Get()
        {
            return await Get<Genero, GeneroDTO>();
        }

        [HttpGet("{id:int}", Name ="ObtenerGenero")]
        public async Task<ActionResult<GeneroDTO>> Get(int id)
        {
            return await Get<Genero, GeneroDTO>(id);
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>( Paginacion paginacionDTO,
           IQueryable<TEntidad> queryable)
           where TEntidad : class
        {
            await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDTO.cantidadRegistosPagina);
            var entidades = await queryable.Paginar(paginacionDTO).ToListAsync();
            return _mapper.Map<List<TDTO>>(entidades);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCrearDTO GeneroDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { ModelState.Values });
            }
            return await Post<GeneroCrearDTO, Genero, GeneroDTO>(GeneroDTO, "ObtenerGenero");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult>Put(int id, [FromBody]GeneroCrearDTO GeneroDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { ModelState.Values });
            }
            return await Put<GeneroCrearDTO, Genero>(id, GeneroDto);
        }

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult>Delete(int id)
        {
            return await Delete<Genero>(id);
        }
    }
}
