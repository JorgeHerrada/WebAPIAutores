using System.ComponentModel.DataAnnotations;

namespace WebAppAutores.DTOs
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
