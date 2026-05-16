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
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuariosService _service;
        public UsuariosController(IUsuariosService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")] // Solo los admin pueden listar usuarios
        public async Task<ActionResult<IEnumerable<UsuariosDto>>> ObtenerTodos([FromQuery] int pagina = 1, [FromQuery] int tamano = 10)
        {
            
            var registros = await _service.ObtenerUsuariosAsync(pagina, tamano);
            var total = await _service.ContarAsync();

            return Ok(new RespuestaPaginada<UsuariosDto>(registros, total, pagina, tamano));
        }

        [HttpGet("{id:int}", Name = "ObtenerUsuario")] 
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UsuariosDto>> ObtenerUsuario(int id)
        {
            var registro = await _service.ObtenerPorIdAsync(id);
            return Ok(registro);
        }
    }
}

