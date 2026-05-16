
using TicketSoporte.Application.DTOs.Usuarios;

namespace TicketSoporte.Application.Response
{
    public class RespuestaLoginDto
    {
        public UsuariosDto Usuario { get; set; } = null!;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiraEn { get; set; }
    }
}
