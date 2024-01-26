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
            CreateMap<Autor, AutorDTOConLibros>()
                .ForMember(autorDTO => autorDTO.Libros, options => options.MapFrom(MapAutorDTOLibros));

            CreateMap<LibroCreationDTO, Libro>()
                .ForMember(libro => libro.AutoresLibros, options => options.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroDTO>();
            CreateMap<Libro, LibroDTOConAutores>()
                .ForMember(libroDTO => libroDTO.Autores, options => options.MapFrom(MapLibroDTOAutores));

            CreateMap<ComentarioCreationDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();
        }

        private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTO autorDTO)
        {
            var result = new List<LibroDTO>();

            if(autor.AutoresLibros == null) { return result; }

            foreach (var autorLibro in autor.AutoresLibros)
            {
                result.Add(new LibroDTO()
                {
                    Id = autorLibro.LibroId,
                    Titulo = autorLibro.Libro.Titulo
                });
            }

            return result;
        }

        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var result = new List<AutorDTO>();

            if (libro.AutoresLibros == null) 
            {
                return result;
            }

            foreach (var autorlibro in libro.AutoresLibros)
            {
                result.Add(new AutorDTO()
                {
                    Id = autorlibro.AutorId,
                    Nombre = autorlibro.Autor.Nombre
                });
                
            }

            return result;
        }

        private List<AutorLibro> MapAutoresLibros(LibroCreationDTO libroCreationDTO, Libro libro)
        {
            var result = new List<AutorLibro>();

            if(libroCreationDTO.AutoresIds == null)
            {
                return result;
            }

            foreach (var autorId in libroCreationDTO.AutoresIds)
            {
                result.Add(new AutorLibro()
                {
                    AutorId = autorId
                });
            }

            return result;
        }
    }
}
