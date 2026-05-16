

using TicketSoporte.Application.DTOs.Departamentos;

namespace TicketSoporte.Application.Interface.Service
{
    public interface IDepartamentoService
    {
        Task<DepartamentosDto?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<DepartamentosDto>> ObtenerDepartamentosDtosAsync(int pagina, int tamano);
        Task<IEnumerable<DepartamentosDto>> BuscarDepartamentosDtosAsync(string valor, int pagina, int tamano);
        Task<int> ContarAsync();
        Task<int> ContarBusquedaAsync(string valor);

     
        Task<DepartamentosDto> CrearAsync(DepartamentosCrearDto dto);
        Task<DepartamentosDto> ActualizarAsync(int id, DepartamentosEditarDto dto);
    }
}
