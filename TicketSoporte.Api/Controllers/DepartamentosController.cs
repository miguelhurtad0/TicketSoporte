using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketSoporte.Application.DTOs.Departamentos;
using TicketSoporte.Application.Interface.Service;
using TicketSoporte.Application.Response;

namespace TicketSoporte.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartamentosController : ControllerBase
    {
        private readonly IDepartamentoService _service;
        private readonly IMapper _mapper;

        public DepartamentosController(IDepartamentoService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartamentosDto>>> ObtenerTodos([FromQuery] int pagina = 1, [FromQuery] int tamano = 10)
        {
            var registros = await _service.ObtenerDepartamentosDtosAsync(pagina, tamano);
            var total = await _service.ContarAsync();

            return Ok(new RespuestaPaginada<DepartamentosDto>(registros, total, pagina, tamano));
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<DepartamentosDto>>> Buscar([FromQuery] string valor, [FromQuery] int pagina = 1, [FromQuery] int tamano = 10)
        {
            var registros = await _service.BuscarDepartamentosDtosAsync(valor, pagina, tamano);
            var total = await _service.ContarBusquedaAsync(valor);

            return Ok(new RespuestaPaginada<DepartamentosDto>(registros, total, pagina, tamano));
        }

        [HttpGet("{id:int}", Name = "ObtenerDepartamento")]
        public async Task<ActionResult<DepartamentosDto>> ObtenerPorId(int id)
        {
            var registro = await _service.ObtenerPorIdAsync(id);

            if (registro == null)
                return NotFound(new { mensaje = $"El departamento con ID {id} no existe." });

            return Ok(registro);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DepartamentosDto>> Crear([FromBody] DepartamentosCrearDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var departamentoCreado = await _service.CrearAsync(dto);


            return CreatedAtRoute("ObtenerDepartamento", new { id = departamentoCreado.Id }, departamentoCreado);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DepartamentosDto>> Editar(int id, [FromBody] DepartamentosEditarDto dto)
        {
            var actualizado = await _service.ActualizarAsync(id, dto);

            if (actualizado == null)
                return NotFound(new { mensaje = "No se pudo actualizar, departamento no encontrado." });

            return Ok(actualizado);
        }
    }
}
