using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketSoporte.Application.DTOs.Usuarios;
using TicketSoporte.Application.Interface.Service;
using TicketSoporte.Application.Response;
using TicketSoporte.Domain.Entites;

namespace TicketSoporte.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuariosService _service;
        private readonly UserManager<Usuarios> _userManager;

        public UsuariosController(IUsuariosService service, UserManager<Usuarios> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        private int ObtenerUsuarioId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        private string ObtenerUsuarioRol() => User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RespuestaPaginada<UsuariosDto>>> ObtenerTodos([FromQuery] int pagina = 1, [FromQuery] int tamano = 10, [FromQuery] string? buscar = null)
        {
            IEnumerable<UsuariosDto> registros;
            int total;

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                registros = await _service.BuscarPorNombreAsync(buscar, pagina, tamano);
                total = await _service.ContarBusquedaAsync(buscar);
            }
            else
            {
                registros = await _service.ObtenerUsuariosAsync(pagina, tamano);
                total = await _service.ContarAsync();
            }

            return Ok(new RespuestaPaginada<UsuariosDto>(registros, total, pagina, tamano));
        }

        [HttpGet("{id:int}", Name = "ObtenerUsuario")]
        public async Task<ActionResult<UsuariosDto>> ObtenerUsuario(int id)
        {
            var rolActual = ObtenerUsuarioRol();
            var idActual = ObtenerUsuarioId();

            if (rolActual != "Admin" && id != idActual)
            {
                return Forbid();
            }

            var registro = await _service.ObtenerPorIdAsync(id);
            return Ok(registro);
        }

        [HttpPut("actualizar-perfil")]
        public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarPerfilDto dto)
        {
            var idActual = ObtenerUsuarioId();
            var usuario = await _userManager.FindByIdAsync(idActual.ToString());

            if (usuario == null)
            {
                return NotFound(new { Detail = "Usuario no encontrado." });
            }

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) && dto.PhoneNumber != usuario.PhoneNumber)
            {
                var resultadoTel = await _userManager.SetPhoneNumberAsync(usuario, dto.PhoneNumber);
                if (!resultadoTel.Succeeded) return BadRequest(new { Detail = "Error al actualizar el teléfono." });
            }

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != usuario.Email)
            {
                var resultadoEmail = await _userManager.SetEmailAsync(usuario, dto.Email);
                var resultadoUsuario = await _userManager.SetUserNameAsync(usuario, dto.Email);

                if (!resultadoEmail.Succeeded || !resultadoUsuario.Succeeded)
                {
                    return BadRequest(new { Detail = "Error al actualizar el correo electrónico." });
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.NombreCompleto) && dto.NombreCompleto != usuario.NombreCompleto)
            {
                usuario.NombreCompleto = dto.NombreCompleto;
                var resultadoNombre = await _userManager.UpdateAsync(usuario);
                if (!resultadoNombre.Succeeded) return BadRequest(new { Detail = "Error al actualizar el nombre." });
            }

            return Ok(new { Mensaje = "Perfil actualizado correctamente." });
        }

        // ¡NUEVO! Endpoint exclusivo para que el Admin actualice a cualquier usuario
        [HttpPut("{id:int}/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActualizarUsuarioPorAdmin(int id, [FromBody] ActualizarUsuarioAdminDto dto)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());

            if (usuario == null)
            {
                return NotFound(new { Detail = "Usuario no encontrado." });
            }

            // 1. Actualizar datos básicos
            usuario.NombreCompleto = dto.NombreCompleto;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) && dto.PhoneNumber != usuario.PhoneNumber)
                await _userManager.SetPhoneNumberAsync(usuario, dto.PhoneNumber);

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != usuario.Email)
            {
                await _userManager.SetEmailAsync(usuario, dto.Email);
                await _userManager.SetUserNameAsync(usuario, dto.Email);
            }

            var resultadoUpdate = await _userManager.UpdateAsync(usuario);
            if (!resultadoUpdate.Succeeded)
            {
                return BadRequest(new { Detail = "Error al guardar los datos básicos del usuario." });
            }

            // 2. Actualizar el rol
            var rolesActuales = await _userManager.GetRolesAsync(usuario);
            if (!rolesActuales.Contains(dto.Rol))
            {
                await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
                var resultadoRol = await _userManager.AddToRoleAsync(usuario, dto.Rol);

                if (!resultadoRol.Succeeded)
                    return BadRequest(new { Detail = "Error al actualizar el rol del usuario." });
            }

            // 3. Actualizar la contraseña si se proporcionó una nueva
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                var resultadoPassword = await _userManager.ResetPasswordAsync(usuario, token, dto.Password);

                if (!resultadoPassword.Succeeded)
                {
                    return BadRequest(new { Detail = "Error al actualizar la contraseña. Asegúrese de que cumpla con los requisitos mínimos." });
                }
            }

            return Ok(new { Mensaje = "Usuario actualizado correctamente por el Administrador." });
        }

        // ¡MODIFICADO! Endpoint para eliminar usuario (Con captura de llaves foráneas y protección del Admin)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var idActual = ObtenerUsuarioId();

            // Medida de seguridad: Un admin no puede eliminarse a sí mismo
            if (id == idActual)
            {
                return BadRequest(new { Detail = "No puedes eliminar tu propia cuenta de administrador." });
            }

            try
            {
                await _service.EliminarUsuarioAsync(id);
                return Ok(new { Mensaje = "Usuario eliminado correctamente." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Detail = "Usuario no encontrado." });
            }
            catch (Exception ex)
            {
                // Captura específica para el error de Base de Datos (Entity Framework - REFERENCE constraint)
                if (ex.InnerException != null && (ex.InnerException.Message.Contains("REFERENCE constraint") || ex.InnerException.Message.Contains("conflicted with the REFERENCE")))
                {
                    return BadRequest(new { Detail = "No se puede eliminar el usuario porque tiene tickets o departamentos asociados. Considere desactivarlo." });
                }

                return BadRequest(new { Detail = "Ocurrió un error interno al intentar eliminar el usuario." });
            }
        }
    }
}

