using System;
using System.Collections.Generic;
using System.Text;

namespace TicketSoporte.Application.DTOs.Usuarios
{
    public class ActualizarPerfilDto
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? NombreCompleto { get; set; }
    }
}
