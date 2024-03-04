namespace WebAppAutores.DTOs
{
    public class ColeccionDeRecursosDTO<T> : RecursoDTO where T : RecursoDTO
    {
        public List<T> Values { get; set; }
    }
}
