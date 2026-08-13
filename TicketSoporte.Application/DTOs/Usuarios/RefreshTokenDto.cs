
using System.ComponentModel.DataAnnotations;

namespace TicketSoporte.Application.DTOs.Usuarios
{
    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
