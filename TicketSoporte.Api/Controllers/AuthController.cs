using Microsoft.AspNetCore.Authorization;
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

            dto.Rol = "Cliente";

            try
            {
                var registroCreado = await _service.RegistrarUsuarioAsync(dto);
                return Ok(registroCreado);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { Detail = ex.Message, Type = "BadRequest", Status = 400 });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Detail = ex.Message, Type = "ServerError", Status = 500 });
            }
        }

        [HttpPost("registro-interno")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UsuariosDto>> RegistroInterno([FromBody] UsuariosRegistroDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.Rol))
                return StatusCode(StatusCodes.Status400BadRequest, new { Detail = "Debes especificar el rol para crear personal.", Type = "BadRequest", Status = 400 });

            try
            {
                var registroCreado = await _service.RegistrarUsuarioAsync(dto);
                return Ok(registroCreado);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { Detail = ex.Message, Type = "BadRequest", Status = 400 });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Detail = ex.Message, Type = "ServerError", Status = 500 });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaLoginDto>> Login([FromBody] UsuariosLoginDto dto)
        {
            try
            {
                var respuesta = await _service.LoginAsync(dto);
                return Ok(respuesta);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { Detail = ex.Message, Type = "Unauthorized", Status = 401 });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Detail = ex.Message, Type = "ServerError", Status = 500 });
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<RespuestaLoginDto>> Refresh([FromBody] RefreshTokenDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken))
                return StatusCode(StatusCodes.Status400BadRequest, new { Detail = "El Refresh Token es requerido.", Type = "BadRequest", Status = 400 });

            try
            {
                var respuesta = await _service.RefreshTokenAsync(dto.RefreshToken);
                return Ok(respuesta);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { Detail = ex.Message, Type = "Unauthorized", Status = 401 });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Detail = ex.Message, Type = "ServerError", Status = 500 });
            }
        }
    }
}
