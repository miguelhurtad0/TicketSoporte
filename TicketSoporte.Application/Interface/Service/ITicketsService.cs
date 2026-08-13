
using TicketSoporte.Application.DTOs.Tickets;

namespace TicketSoporte.Application.Interface.Service
{
     public interface ITicketsService
    {
        Task<TicketsDto?> ObtenerPorIdAsync(int id);

        Task<IEnumerable<TicketsDto>> ObtenerTicketsDtosAsync(int pagina, int tamano);
        Task<IEnumerable<TicketsDto>> BuscarTicketsDtosAsync(string valor, int pagina, int tamano);
        Task<IEnumerable<TicketsDto>> ObtenerPorDepartamentoAsync(int departamentoId, int pagina, int tamano);
        Task<IEnumerable<TicketsDto>> ObtenerPorClienteAsync(int clienteId, int pagina, int tamano);
        Task<IEnumerable<TicketsDto>> ObtenerPorTecnicoAsync(int tecnicoId, int pagina, int tamano);

        Task<int> ContarPorTecnicoAsync(int tecnicoId);
        Task<int> ContarPorClienteAsync(int clienteId);
        Task<int> ContarPorDepartamentoAsync(int departamentoId);
        Task<int> ContarAsync();
        Task<int> ContarBusquedaAsync(string valor);

        Task<TicketsDto> CrearAsync(TicketsCrearDto dto);
        Task<TicketsDto> ActualizarAsync(int id, TicketsEditarDto dto);
        Task CambiarEstadoAsync(int id, string nuevoEstado);
    }
}
