﻿using System.ComponentModel.DataAnnotations;

namespace WebAppAutores.DTOs
{
    public class CredencialesUsuario
    {
        // information that we get from the user to loggin
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
