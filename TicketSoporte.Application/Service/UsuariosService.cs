
using AutoMapper;
using TicketSoporte.Application.DTOs.Usuarios;
using TicketSoporte.Application.Interface.Repository;
using TicketSoporte.Application.Interface.Service;

namespace TicketSoporte.Application.Service
{
    public class UsuariosService : IUsuariosService
    {
        private readonly IUsuariosRepository _repository;
        private readonly IMapper _mapper;

        public UsuariosService(IUsuariosRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<int> ContarAsync()
        {
            return await _repository.ContarAsync();
        }

        public async Task<UsuariosDto?> ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID es requerido.");
            }

            var registro = await _repository.ObtenerPorIdAsync(id);

            if (registro == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado.");
            }

            return _mapper.Map<UsuariosDto>(registro);
        }

        public async Task<IEnumerable<UsuariosDto>> ObtenerUsuariosAsync(int pagina, int tamano)
        {
            var registros = await _repository.ObtenerUsuariosAsync(pagina, tamano);
            return _mapper.Map<IEnumerable<UsuariosDto>>(registros);
        }
    }
}
