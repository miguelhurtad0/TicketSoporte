
using AutoMapper;
using TicketSoporte.Application.DTOs.Comentarios;
using TicketSoporte.Application.Interface.Repository;
using TicketSoporte.Application.Interface.Service;
using TicketSoporte.Domain.Entites;

namespace TicketSoporte.Application.Service
{
    public class ComentariosService : IComentariosService
    {
        private readonly IComentariosRepository _repository;
        private readonly IMapper _mapper;

        public ComentariosService(IComentariosRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ComentariosDto> ActualizarAsync(int id, ComentariosEditarDto dto)
        {
            var comentarioExistente = await _repository.ObtenerPorIdAsync(id);
            if (comentarioExistente == null) return null!;
            _mapper.Map(dto, comentarioExistente);
            await _repository.ActualizarAsync(comentarioExistente);
            return _mapper.Map<ComentariosDto>(comentarioExistente);
        }

        public async Task<IEnumerable<ComentariosDto>> BuscarComentariosDtosAsync(string valor, int pagina, int tamano)
        {
            var registros = await _repository.BuscarComentariosAsync(valor, pagina, tamano);

            return _mapper.Map<IEnumerable<ComentariosDto>>(registros);
        }

        public async Task<int> ContarAsync()
        {
            return await _repository.ContarAsync();
        }

        public async Task<int> ContarBusquedaAsync(string valor)
        {
            return await _repository.ContarBusquedaAsync(valor);
        }

        public async Task<ComentariosDto> CrearAsync(ComentariosCrearDto dto)
        {
            var comentarioEntidad = _mapper.Map<Comentarios>(dto);
            await _repository.CrearAsync(comentarioEntidad);
            return _mapper.Map<ComentariosDto>(comentarioEntidad);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            // Verificamos si existe antes de intentar borrar
            var existe = await _repository.ObtenerPorIdAsync(id);
            if (existe == null) return false;

            await _repository.EliminarAsync(id);
            return true;
        }

        public async Task<IEnumerable<ComentariosDto>> ObtenerComentariosDtosAsync(int pagina, int tamano)
        {
            var comentarios = await _repository.ObtenerComentariosAsync(pagina, tamano);
            return _mapper.Map<IEnumerable<ComentariosDto>>(comentarios);
        }

        public async Task<ComentariosDto?> ObtenerPorIdAsync(int id)
        {
            var comentario = await _repository.ObtenerPorIdAsync(id);
            return _mapper.Map<ComentariosDto>(comentario);
        }

        public async Task<IEnumerable<ComentariosDto>> ObtenerPorTicketIdAsync(int ticketId)
        {
            var comentarios = await _repository.ObtenerPorTicketIdAsync(ticketId);
            return _mapper.Map<IEnumerable<ComentariosDto>>(comentarios);
        }
    }
}
