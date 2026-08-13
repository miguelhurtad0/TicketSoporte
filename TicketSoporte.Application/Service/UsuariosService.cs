using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TicketSoporte.Application.DTOs.Usuarios;
using TicketSoporte.Application.Interface.Repository;
using TicketSoporte.Application.Interface.Service;
using TicketSoporte.Domain.Entites;

namespace TicketSoporte.Application.Service
{
    public class UsuariosService : IUsuariosService
    {
        private readonly IUsuariosRepository _repository;
        private readonly IMapper _mapper;
        private readonly UserManager<Usuarios> _userManager;

        public UsuariosService(IUsuariosRepository repository, IMapper mapper, UserManager<Usuarios> userManager)
        {
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<int> ContarAsync()
        {
            return await _repository.ContarAsync();
        }

        public async Task<UsuariosDto?> ObtenerPorIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID es requerido.");

            var registro = await _repository.ObtenerPorIdAsync(id);
            if (registro == null) throw new KeyNotFoundException("Usuario no encontrado.");

            var dto = _mapper.Map<UsuariosDto>(registro);

            var userIdentity = await _userManager.FindByIdAsync(dto.Id.ToString());
            if (userIdentity != null)
            {
                var roles = await _userManager.GetRolesAsync(userIdentity);
                dto.Rol = roles.FirstOrDefault() ?? string.Empty;
            }

            return dto;
        }

        public async Task<IEnumerable<UsuariosDto>> ObtenerUsuariosAsync(int pagina, int tamano)
        {
            var registros = await _repository.ObtenerUsuariosAsync(pagina, tamano);
            var dtos = _mapper.Map<IEnumerable<UsuariosDto>>(registros).ToList();

            foreach (var dto in dtos)
            {
                var userIdentity = await _userManager.FindByIdAsync(dto.Id.ToString());
                if (userIdentity != null)
                {
                    var roles = await _userManager.GetRolesAsync(userIdentity);
                    dto.Rol = roles.FirstOrDefault() ?? string.Empty;
                }
            }

            return dtos;
        }

        public async Task<int> ContarBusquedaAsync(string nombre)
        {
            return await _repository.ContarBusquedaAsync(nombre);
        }


        public async Task<IEnumerable<UsuariosDto>> BuscarPorNombreAsync(string nombre, int pagina, int tamano)
        {
            var registros = await _repository.BuscarPorNombreAsync(nombre, pagina, tamano);
            var dtos = _mapper.Map<IEnumerable<UsuariosDto>>(registros).ToList();

            foreach (var dto in dtos)
            {
                var userIdentity = await _userManager.FindByIdAsync(dto.Id.ToString());
                if (userIdentity != null)
                {
                    var roles = await _userManager.GetRolesAsync(userIdentity);
                    dto.Rol = roles.FirstOrDefault() ?? string.Empty;
                }
            }
            return dtos;
        }

        public async Task<bool> EliminarUsuarioAsync(int id)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());

            if (usuario == null)
                throw new KeyNotFoundException("Usuario no encontrado.");

            var resultado = await _userManager.DeleteAsync(usuario);

            if (!resultado.Succeeded)
            {
                var errores = string.Join(" | ", resultado.Errors.Select(e => e.Description));
                throw new Exception(errores);
            }

            return true;
        }
    }
}