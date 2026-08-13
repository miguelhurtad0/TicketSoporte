using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Diagnostics.Metrics;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Xml.Linq;
using TicketSoporte.Api.Request;
using TicketSoporte.Application.DTOs.Tickets;
using TicketSoporte.Application.Interface.Service;
using TicketSoporte.Application.Response;
using TicketSoporte.Application.Service;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TicketSoporte.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketsService _service;
        private readonly IMapper _mapper;

        public TicketsController(ITicketsService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        
        private int ObtenerUsuarioId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        private string ObtenerUsuarioRol() => User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;


        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketsDto>>> ObtenerTodos([FromQuery] int pagina = 1, [FromQuery] int tamano = 10)
        {
            var rol = ObtenerUsuarioRol();
            var usuarioId = ObtenerUsuarioId();

            
            if (rol == "Admin")
            {
                var registros = await _service.ObtenerTicketsDtosAsync(pagina, tamano);
                var total = await _service.ContarAsync();
                return Ok(new RespuestaPaginada<TicketsDto>(registros, total, pagina, tamano));
            }
            else if (rol == "Tecnico")
            {
               
                var registros = await _service.ObtenerPorTecnicoAsync(usuarioId, pagina, tamano);
                var total = await _service.ContarPorTecnicoAsync(usuarioId);
                return Ok(new RespuestaPaginada<TicketsDto>(registros, total, pagina, tamano));
            }
            else 
            {
                
                var registros = await _service.ObtenerPorClienteAsync(usuarioId, pagina, tamano);
                var total = await _service.ContarPorClienteAsync(usuarioId);
                return Ok(new RespuestaPaginada<TicketsDto>(registros, total, pagina, tamano));
            }
        }

        [HttpGet("{id:int}", Name = "ObtenerTicket")]
        public async Task<ActionResult<TicketsDto>> ObtenerTicket(int id)
        {
            var registro = await _service.ObtenerPorIdAsync(id);

            if (registro == null)
                return NotFound(new { mensaje = $"El ticket con ID {id} no existe." });

            var rol = ObtenerUsuarioRol();
            var usuarioId = ObtenerUsuarioId();

            // Validamos si el usuario es el dueño de la información
            if (rol == "Cliente" && registro.ClienteId != usuarioId)
                return Forbid(); // HTTP 403: Prohibido. El ticket existe, pero no es tuyo.

            if (rol == "Tecnico" && registro.TecnicoAsignadoId != usuarioId)
                return Forbid(); // Un técnico no puede fisgonear los tickets de otros técnicos.

            return Ok(registro);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Cliente")] // Los técnicos generalmente no abren tickets a su nombre
        public async Task<ActionResult<TicketsDto>> Crear([FromBody] TicketsCrearRequest request)
        {
            var rol = ObtenerUsuarioRol();
            var usuarioId = ObtenerUsuarioId();

            
            int idClienteReal = (rol == "Cliente") ? usuarioId : request.ClienteId;

            var nuevoTicket = new TicketsCrearDto
            {
                Asunto = request.Asunto,
                Descripcion = request.Descripcion,
                NumeroSerieEquipo = request.NumeroSerieEquipo,
                ClienteId = idClienteReal, 
                DepartamentoId = request.DepartamentoId
            };

            var TicketCreado = await _service.CrearAsync(nuevoTicket);

            return CreatedAtRoute("ObtenerTicket", new { id = TicketCreado.Id }, TicketCreado);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Tecnico")] // Un cliente no debería editar el ticket completo, solo agregar comentarios o cambiar estado
        public async Task<ActionResult<TicketsDto>> Editar(int id, [FromBody] TicketsEditarRequest request)
        {
            var rol = ObtenerUsuarioRol();
            var usuarioId = ObtenerUsuarioId();

            
            if (rol == "Tecnico")
            {
                var ticketActual = await _service.ObtenerPorIdAsync(id);
                if (ticketActual == null) return NotFound();
                if (ticketActual.TecnicoAsignadoId != usuarioId) return Forbid();
            }

            var TicketActualizado = new TicketsEditarDto
            {
                Asunto = request.Asunto,
                Descripcion = request.Descripcion,
                NumeroSerieEquipo = request.NumeroSerieEquipo,
                ClienteId = request.ClienteId,
                DepartamentoId = request.DepartamentoId,
                TecnicoAsignadoId = request.TecnicoAsignadoId
            };

            var Actualizado = await _service.ActualizarAsync(id, TicketActualizado);

            return Ok(Actualizado);
        }

        [HttpPatch("{id}/estado")]
        
        [Authorize(Roles = "Admin,Tecnico,Cliente")]
        public async Task<ActionResult> CambiarEstado(int id, [FromBody] string nuevoEstado)
        {
            if (string.IsNullOrWhiteSpace(nuevoEstado))
                return BadRequest("El estado es requerido.");

            var rol = ObtenerUsuarioRol();
            var usuarioId = ObtenerUsuarioId();

            // Validar propiedad del ticket antes de cambiar el estado
            var ticketActual = await _service.ObtenerPorIdAsync(id);
            if (ticketActual == null) return NotFound();

            if (rol == "Cliente" && ticketActual.ClienteId != usuarioId) return Forbid();
            if (rol == "Tecnico" && ticketActual.TecnicoAsignadoId != usuarioId) return Forbid();

            await _service.CambiarEstadoAsync(id, nuevoEstado);

            return NoContent();
        }
    }
}
