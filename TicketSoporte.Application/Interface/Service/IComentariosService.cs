

using TicketSoporte.Application.DTOs.Comentarios;
using TicketSoporte.Application.DTOs.Tickets;

namespace TicketSoporte.Application.Interface.Service
{
    public interface IComentariosService
    {
        Task<ComentariosDto?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<ComentariosDto>> ObtenerComentariosDtosAsync(int pagina, int tamano);
        Task<IEnumerable<ComentariosDto>> BuscarComentariosDtosAsync(string valor, int pagina, int tamano);
        Task<IEnumerable<ComentariosDto>> ObtenerPorTicketIdAsync(int ticketId, bool ocultarInternos);
        Task<int> ContarAsync();
        Task<int> ContarBusquedaAsync(string valor);

        Task<ComentariosDto> CrearAsync(ComentariosCrearDto dto);
        Task<ComentariosDto> ActualizarAsync(int id, ComentariosEditarDto dto);
        Task<bool> EliminarAsync(int id);

        Task<IEnumerable<ComentariosDto>> ObtenerPorTicketIdAsync(int ticketId);
    }
}
