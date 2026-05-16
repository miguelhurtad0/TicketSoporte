

using AutoMapper;
using Microsoft.Win32;
using TicketSoporte.Application.DTOs.Tickets;
using TicketSoporte.Application.Interface.Repository;
using TicketSoporte.Application.Interface.Service;
using TicketSoporte.Domain.Entites;

namespace TicketSoporte.Application.Service
{
    public class TicketsService : ITicketsService
    {
        private readonly ITicketsRepository _repository;
        private readonly IMapper _mapper;

        public TicketsService(ITicketsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TicketsDto> ActualizarAsync(int id, TicketsEditarDto dto)
        {
            var ticketExistente = await _repository.ObtenerPorIdAsync(id);
            if (ticketExistente == null) return null!;

            _mapper.Map(dto, ticketExistente);

            await _repository.ActualizarAsync(ticketExistente);
            return _mapper.Map<TicketsDto>(ticketExistente);
            
        }

        public async Task<IEnumerable<TicketsDto>> BuscarTicketsDtosAsync(string valor, int pagina, int tamano)
        {
            var tickets = await _repository.BuscarTicketsAsync(valor, pagina, tamano);
            return _mapper.Map<IEnumerable<TicketsDto>>(tickets);
        }

        public async Task CambiarEstadoAsync(int id, string nuevoEstado)
        {
             
            if (id <= 0)
                throw new ArgumentException("El ID del producto debe ser mayor que cero.", nameof(id));

            var ticket = await _repository.ObtenerPorIdAsync(id);
            if (ticket == null)
                throw new KeyNotFoundException("Registro no encontrado.");

            await _repository.CambiarEstadoAsync(id, nuevoEstado);
        }

        public async Task<int> ContarAsync()
        {
          return await _repository.ContarAsync();
        }

        public async Task<int> ContarBusquedaAsync(string valor)
        {
           return await _repository.ContarBusquedaAsync(valor);
        }

        public async Task<int> ContarPorClienteAsync(int clienteId)
        {
            return await _repository.ContarPorDepartamentoAsync(clienteId);
        }

        public async Task<int> ContarPorDepartamentoAsync(int departamentoId)
        {
            return await _repository.ContarPorDepartamentoAsync(departamentoId);
        }

        public async Task<TicketsDto> CrearAsync(TicketsCrearDto dto)
        {
            var entidad = _mapper.Map<Tickets>(dto);

            
            entidad.Estado = "Abierto";
            await _repository.CrearAsync(entidad);
            return _mapper.Map<TicketsDto>(entidad);
        }

        public async Task<IEnumerable<TicketsDto>> ObtenerPorClienteAsync(int clienteId, int pagina, int tamano)
        {
            var tickets = await _repository.ObtenerPorClienteAsync(clienteId, pagina, tamano);
            return _mapper.Map<IEnumerable<TicketsDto>>(tickets);
        }

        public async Task<IEnumerable<TicketsDto>> ObtenerPorDepartamentoAsync(int departamentoId, int pagina, int tamano)
        {
            var tickets = await _repository.ObtenerPorDepartamentoAsync(departamentoId, pagina, tamano);
            return _mapper.Map<IEnumerable<TicketsDto>>(tickets);
        }

        public async Task<TicketsDto?> ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del producto debe ser mayor que cero.", nameof(id));

            var ticket = await _repository.ObtenerPorIdAsync(id);
            if (ticket == null)
                throw new KeyNotFoundException("Registro no encontrado.");

            return _mapper.Map<TicketsDto>(ticket);
        }

        public async Task<IEnumerable<TicketsDto>> ObtenerTicketsDtosAsync(int pagina, int tamano)
        {
            var tickets = await _repository.ObtenerTicketsAsync(pagina, tamano);
            return _mapper.Map<IEnumerable<TicketsDto>>(tickets);
        }
    }
}
