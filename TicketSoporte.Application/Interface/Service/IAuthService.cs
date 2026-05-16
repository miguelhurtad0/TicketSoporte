

using TicketSoporte.Application.DTOs.Usuarios;
using TicketSoporte.Application.Response;

namespace TicketSoporte.Application.Interface.Service
{
    public interface IAuthService
    {
        Task<RespuestaLoginDto> LoginAsync(UsuariosLoginDto dto);
        Task<UsuariosDto> RegistrarUsuarioAsync(UsuariosRegistroDto dto);
        Task<RespuestaLoginDto> RefreshTokenAsync(string refreshToken);
    }
}
