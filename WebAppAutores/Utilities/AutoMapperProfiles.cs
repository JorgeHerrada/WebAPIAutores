using AutoMapper;
using WebAppAutores.Controllers.Entidades;
using WebAppAutores.DTOs;

namespace WebAppAutores.Utilities
{
    // class that maps properties of 2 different classes 
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            // source -> destination
            CreateMap<AutorCreationDTO, Autor>();
            CreateMap<Autor, AutorDTO>();
            CreateMap<LibroCreationDTO, Libro>();
            CreateMap<Libro, LibroDTO>();
        }
    }
}
