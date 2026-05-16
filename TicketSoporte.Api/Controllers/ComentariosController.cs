using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketSoporte.Api.Request;
using TicketSoporte.Application.DTOs.Comentarios;
using TicketSoporte.Application.Interface.Service;
using TicketSoporte.Application.Response;

namespace TicketSoporte.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private readonly IComentariosService _service;
        private readonly IMapper _mapper;

        public ComentariosController(IComentariosService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Tecnico")]
        public async Task<ActionResult<IEnumerable<ComentariosDto>>> ObtenerTodos([FromQuery] int pagina = 1, [FromQuery] int tamano = 10)
        {
            var registros = await _service.ObtenerComentariosDtosAsync(pagina, tamano);
            var total = await _service.ContarAsync();
            return Ok(new RespuestaPaginada<ComentariosDto>(registros, total, pagina, tamano));
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<ComentariosDto>>> Buscar([FromQuery] string valor, [FromQuery] int pagina = 1, [FromQuery] int tamano = 10)
        {
            var registros = await _service.BuscarComentariosDtosAsync(valor, pagina, tamano);
            var total = await _service.ContarBusquedaAsync(valor);
            return Ok(new RespuestaPaginada<ComentariosDto>(registros, total, pagina, tamano));
        }

        [HttpGet("{id:int}", Name = "ObtenerComentario")]
        public async Task<ActionResult<ComentariosDto>> ObtenerPorId(int id)
        {
            var registro = await _service.ObtenerPorIdAsync(id);
     
            if (registro == null)
                return NotFound(new { mensaje = $"El comentario con ID {id} no existe." });

            return Ok(registro);
        }

        // Método especial para ver el historial de un ticket
        [HttpGet("ticket/{ticketId:int}")]
        public async Task<ActionResult<IEnumerable<ComentariosDto>>> ObtenerPorTicket(int ticketId)
        {
            var registros = await _service.ObtenerPorTicketIdAsync(ticketId);
            return Ok(registros);
        }

        [HttpPost]
        public async Task<ActionResult<ComentariosDto>> Crear([FromBody] ComentariosCrearDto dto)
        {
            var creado = await _service.CrearAsync(dto);

            return CreatedAtRoute("ObtenerComentario", new { id = creado.Id }, creado);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ComentariosDto>> Editar(int id, [FromBody] ComentariosEditarDto dto)
        {
            var actualizado = await _service.ActualizarAsync(id, dto);

            if (actualizado == null) return NotFound();

            return Ok(actualizado);
        }
    }

}
