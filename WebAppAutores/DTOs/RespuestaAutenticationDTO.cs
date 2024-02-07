namespace WebAppAutores.DTOs
{
    public class RespuestaAutenticationDTO
    {
        // information that we'll send to the user once logged 
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }
    }
}
