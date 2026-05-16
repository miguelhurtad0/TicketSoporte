using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TicketSoporte.Application.DTOs.Usuarios
{
    public class UsuariosLoginDto
    {
        [Required(ErrorMessage = "El email del usuario es requerido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña del usuario es requerida.")]
        public string Password { get; set; } = string.Empty;
    }
}
