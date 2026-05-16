using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Diagnostics.Metrics;
using System.Runtime.ConstrainedExecution;
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
    public class TicketsController : ControllerBase
    {
        private readonly ITicketsService _service;
        private readonly IMapper _mapper;

        public TicketsController(ITicketsService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketsDto>>> ObtenerTodos([FromQuery]int pagina = 1,[FromQuery] int tamano = 10)
        {
            var registros = await _service.ObtenerTicketsDtosAsync(pagina, tamano);
            var total = await _service.ContarAsync();
            return Ok(new RespuestaPaginada<TicketsDto>(registros, total, pagina, tamano));
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<TicketsDto>>> Buscar([FromQuery] string valor, [FromQuery] int pagina = 1, [FromQuery] int tamano = 10)
        {
            var registros = await _service.BuscarTicketsDtosAsync(valor, pagina, tamano);
            var total = await _service.ContarBusquedaAsync(valor);
            return Ok(new RespuestaPaginada<TicketsDto>(registros, total, pagina, tamano));
        }


        [HttpGet("{id:int}", Name ="ObtenerTicket")]
        public async Task<ActionResult<IEnumerable<TicketsDto>>> ObtenerTicket(int id)
        {
            var registro = await _service.ObtenerPorIdAsync(id);
            
            if (registro == null)
                return NotFound(new { mensaje = $"El ticket con ID {id} no existe." });

            return Ok(registro);
        }

   

        [HttpGet("departamento/{departamentoId:int}")]
        public async Task<ActionResult<IEnumerable<TicketsDto>>> BuscraPorDepartamento(int departamentoId, [FromQuery] int pagina = 1, [FromQuery] int tamano = 10)
        {
            var registros = await _service.ObtenerPorDepartamentoAsync(departamentoId, pagina, tamano);
            var total = await _service.ContarPorDepartamentoAsync(departamentoId);
            return Ok(new RespuestaPaginada<TicketsDto>(registros, total, pagina, tamano));
        }

        [HttpGet("cliente/{clienteId:int}")]
        public async Task<ActionResult<IEnumerable<TicketsDto>>> BuscarPorCliente(int clienteId, [FromQuery] int pagina = 1, [FromQuery] int tamano = 10)
        {
            var registros = await _service.ObtenerPorClienteAsync(clienteId, pagina, tamano);
            var total = await _service.ContarPorClienteAsync(clienteId);
            return Ok(new RespuestaPaginada<TicketsDto>(registros, total, pagina, tamano));
        }

        [HttpPost]
        public async Task<ActionResult<TicketsDto>> Crear([FromBody] TicketsCrearRequest request)
        {
            var nuevoTicket = new TicketsCrearDto
            {
                Asunto = request.Asunto,
                Descripcion = request.Descripcion,
                NumeroSerieEquipo = request.NumeroSerieEquipo,
                ClienteId = request.ClienteId,
                DepartamentoId = request.DepartamentoId
            };
            var TicketCreado = await _service.CrearAsync(nuevoTicket);

            return CreatedAtRoute("ObtenerTicket", new { id = TicketCreado.Id }, TicketCreado);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TicketsDto>> Editar(int id, [FromBody] TicketsEditarRequest request)
        {
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
        [Authorize(Roles = "Admin,Tecnico")]
        public async Task<ActionResult> CambiarEstado(int id, [FromBody] string nuevoEstado)
        {
            if (string.IsNullOrWhiteSpace(nuevoEstado))
                return BadRequest("El estado es requerido.");

            await _service.CambiarEstadoAsync(id, nuevoEstado);

            return NoContent();
            
        }


    }
}
