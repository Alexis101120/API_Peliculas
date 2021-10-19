using API_Peliculas.DTO;
using API_Peliculas.Entidades;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Peliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalasDeCineController : CustomBaseController
    {
        private readonly ApplicationDbContext _contex;
        private readonly IMapper _mapper;

        public SalasDeCineController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _contex = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<SalaDeCineDTO>>> Get()
        {
            return await Get<SalaDeCine, SalaDeCineDTO>();
        }

        [HttpGet("{id:int}", Name = "obtenerSalaDeCine")]
        public async Task<ActionResult<SalaDeCineDTO>> Get(int id)
        {
            return await Get<SalaDeCine, SalaDeCineDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SalaDeCineCrearDTO salaDeCineCreacionDTO)
        {
            return await Post<SalaDeCineCrearDTO, SalaDeCine, SalaDeCineDTO>(salaDeCineCreacionDTO, "obtenerSalaDeCine");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] SalaDeCineCrearDTO salaDeCineCreacionDTO)
        {
            return await Put<SalaDeCineCrearDTO, SalaDeCine>(id,salaDeCineCreacionDTO);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<SalaDeCine>(id);
        }

    }
}
