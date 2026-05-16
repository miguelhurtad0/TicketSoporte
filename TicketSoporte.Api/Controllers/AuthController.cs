using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketSoporte.Application.DTOs.Usuarios;
using TicketSoporte.Application.Interface.Service;
using TicketSoporte.Application.Response;

namespace TicketSoporte.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("registro")]
        public async Task<ActionResult<UsuariosDto>> Registro([FromBody] UsuariosRegistroDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var registroCreado = await _service.RegistrarUsuarioAsync(dto);
            return Ok(registroCreado);
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaLoginDto>> Login([FromBody] UsuariosLoginDto dto)
        {
            var respuesta = await _service.LoginAsync(dto);
            return Ok(respuesta);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<RespuestaLoginDto>> Refresh([FromBody] RefreshTokenDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Refreshtoken))
                return BadRequest("El Refresh Token es requerido.");

            var respuesta = await _service.RefreshTokenAsync(dto.Refreshtoken);
            return Ok(respuesta);
        }
    }
}
