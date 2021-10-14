using API_Peliculas.DTO;
using API_Peliculas.Entidades;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Peliculas.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCrearDTO, Genero>();
            CreateMap<ActorDTO, Actor>().ReverseMap();
            CreateMap<ActorCrearDTO, Actor>().ReverseMap().ForMember(x=> x.Foto, options=> options.Ignore());
            CreateMap<ActorPatchDTO, Actor>().ReverseMap();
            CreateMap<PeliculaDTO, Pelicula>().ReverseMap();
            CreateMap<PeliculaCrearDTO, Pelicula>().ForMember(x=> x.Poster, options=> options.Ignore())
                .ForMember(x=> x.PeliculasGeneros, options=> options.MapFrom(MapPeliculasGeneros))
                .ForMember(x=> x.PeliculasActores, options=> options.MapFrom(MapPeliculasActores)); 
            CreateMap<PeliculaPatchDTO, Pelicula>().ReverseMap();
            CreateMap<Pelicula, PeliculaDetalleDTO>()
                .ForMember(x => x.Generos, options => options.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.Actores, options => options.MapFrom(MapPeliculasActores));
        }

        private List<ActorPeliculaDetalleDTO> MapPeliculasActores(Pelicula pelicula, PeliculaDetalleDTO peliculaDetalleDTO)
        {
            var resultado = new List<ActorPeliculaDetalleDTO>();
            if(pelicula.PeliculasActores == null) { return resultado; }
            foreach(var ActorPelicula in pelicula.PeliculasActores)
            {
                resultado.Add(new ActorPeliculaDetalleDTO
                {
                    ActorId = ActorPelicula.ActorId,
                    Personaje = ActorPelicula.Personaje,
                    NombrePersona = ActorPelicula.Actor.Nombre
                });
            }

            return resultado;
        }

        private List<GeneroDTO> MapPeliculasGeneros(Pelicula pelicula, PeliculaDetalleDTO peliculaDetalleDTO)
        {
            var resultado = new List<GeneroDTO>();
            if(pelicula.PeliculasGeneros == null) { return resultado; }
            foreach(var generoPelicula in pelicula.PeliculasGeneros)
            {
                resultado.Add(new GeneroDTO() { Id = generoPelicula.GeneroId, Nombre = generoPelicula.Genero.Nombre });
            }
            return resultado;
        }
       
        private List<PeliculasGeneros> MapPeliculasGeneros(PeliculaCrearDTO peliculaCrearDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasGeneros>();
            if(peliculaCrearDTO.GenerosIDs == null) { return resultado; }
            foreach(var id in peliculaCrearDTO.GenerosIDs)
            {
                resultado.Add(new PeliculasGeneros() { GeneroId = id });
            }

            return resultado;
        }

        private List<PeliculasActores> MapPeliculasActores(PeliculaCrearDTO peliculaCrearDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();
            if(peliculaCrearDTO.Actores == null)
            {
                return resultado;
            }
            foreach (var actor in peliculaCrearDTO.Actores)
            {
                resultado.Add(new PeliculasActores() { ActorId = actor.ActorId, Personaje = actor.Personaje });
            }
            return resultado;
        }

    }
}
