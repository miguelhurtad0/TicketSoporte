using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketSoporte.Api.Request;
using TicketSoporte.Application.DTOs.Comentarios;
using TicketSoporte.Application.Interface.Service;
using TicketSoporte.Application.Response;

namespace TicketSoporte.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class ComentariosController : ControllerBase
    {
        private readonly IComentariosService _service;
        private readonly ITicketsService _ticketsService; 
        private readonly IMapper _mapper;

        
        public ComentariosController(IComentariosService service, ITicketsService ticketsService, IMapper mapper)
        {
            _service = service;
            _ticketsService = ticketsService;
            _mapper = mapper;
        }

        
        private int ObtenerUsuarioId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        private string ObtenerUsuarioRol() => User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;


        [HttpGet("ticket/{ticketId:int}")]
        public async Task<ActionResult<IEnumerable<ComentariosDto>>> ObtenerPorTicket(int ticketId)
        {
            var rol = ObtenerUsuarioRol();
            var usuarioId = ObtenerUsuarioId();

            var ticket = await _ticketsService.ObtenerPorIdAsync(ticketId);
            if (ticket == null) return NotFound(new { mensaje = "El ticket no existe." });

            if (rol == "Cliente" && ticket.ClienteId != usuarioId) return Forbid();
            if (rol == "Tecnico" && ticket.TecnicoAsignadoId != usuarioId) return Forbid();

           
            bool ocultarInternos = (rol == "Cliente");

            var registros = await _service.ObtenerPorTicketIdAsync(ticketId, ocultarInternos);
            return Ok(registros);
        }

        [HttpPost]
        public async Task<ActionResult<ComentariosDto>> Crear([FromBody] ComentariosCrearDto dto)
        {
            var rol = ObtenerUsuarioRol();
            var usuarioId = ObtenerUsuarioId();

          
            var ticket = await _ticketsService.ObtenerPorIdAsync(dto.TikectId); 
            if (ticket == null) return NotFound(new { mensaje = "El ticket no existe." });

            if (rol == "Cliente" && ticket.ClienteId != usuarioId) return Forbid();
            if (rol == "Tecnico" && ticket.TecnicoAsignadoId != usuarioId) return Forbid();

        
            if (rol == "Cliente") dto.EsInterno = "false"; 

            dto.AutorId = usuarioId; 

            var creado = await _service.CrearAsync(dto);

            return CreatedAtRoute("ObtenerComentario", new { id = creado.Id }, creado);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ComentariosDto>> Editar(int id, [FromBody] ComentariosEditarDto dto)
        {
            var rol = ObtenerUsuarioRol();
            var usuarioId = ObtenerUsuarioId();

            var comentarioActual = await _service.ObtenerPorIdAsync(id);
            if (comentarioActual == null) return NotFound();

            // Seguridad: Solo puedes editar tu propio comentario (a menos que seas Admin)
            if (rol != "Admin" && comentarioActual.AutorId != usuarioId) return Forbid();

            // Seguridad: Un cliente no puede modificar un comentario para hacerlo interno
            if (rol == "Cliente") dto.EsInterno = "false";

            var actualizado = await _service.ActualizarAsync(id, dto);
            return Ok(actualizado);
        }
    }

}
