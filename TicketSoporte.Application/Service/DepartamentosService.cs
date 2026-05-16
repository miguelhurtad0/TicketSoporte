
using AutoMapper;
using TicketSoporte.Application.DTOs.Departamentos;
using TicketSoporte.Application.Interface.Repository;
using TicketSoporte.Application.Interface.Service;
using TicketSoporte.Domain.Entites;

namespace TicketSoporte.Application.Service
{
    public class DepartamentosService : IDepartamentoService
    {
        private readonly IDepartamentosRepository _repository;
        private readonly IMapper _mapper;

        public DepartamentosService(IDepartamentosRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<DepartamentosDto> ActualizarAsync(int id, DepartamentosEditarDto dto)
        {
            var existente = await _repository.ObtenerPorIdAsync(id);
            if (existente == null) return null;
            _mapper.Map(dto, existente);
            await _repository.ActualizarAsync(existente);
            return _mapper.Map<DepartamentosDto>(existente);
        }

        public async Task<IEnumerable<DepartamentosDto>> BuscarDepartamentosDtosAsync(string valor, int pagina, int tamano)
        {
            var deptos = await _repository.BuscarDepartamentosAsync(valor, pagina, tamano);
            return _mapper.Map<IEnumerable<DepartamentosDto>>(deptos);
        }

        public async Task<int> ContarAsync()
        {
            return await _repository.ContarAsync();
        }

        public async Task<int> ContarBusquedaAsync(string valor)
        {
            return await _repository.ContarBusquedaAsync(valor);
        }

        public async Task<DepartamentosDto> CrearAsync(DepartamentosCrearDto dto)
        {
            var entidad = _mapper.Map<Departamentos>(dto);
            await _repository.CrearAsync(entidad);
            return _mapper.Map<DepartamentosDto>(entidad);
        }

        public async Task<IEnumerable<DepartamentosDto>> ObtenerDepartamentosDtosAsync(int pagina, int tamano)
        {
            var deptos = await _repository.ObtenerDepartamentosAsync(pagina, tamano);
            return _mapper.Map<IEnumerable<DepartamentosDto>>(deptos);
        }

        public async Task<DepartamentosDto?> ObtenerPorIdAsync(int id)
        {
            var depto = await _repository.ObtenerPorIdAsync(id);
            return _mapper.Map<DepartamentosDto>(depto);
        }
    }
}
