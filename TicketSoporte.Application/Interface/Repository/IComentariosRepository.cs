
using TicketSoporte.Domain.Entites;

namespace TicketSoporte.Application.Interface.Repository
{
    public interface IComentariosRepository
    {
       
        Task<Comentarios?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Comentarios>> ObtenerComentariosAsync(int numPagina, int cantidad);
        Task<IEnumerable<Comentarios>> BuscarComentariosAsync(string valor, int numPagina, int cantidad);
        Task<int> ContarAsync();
        Task<int> ContarBusquedaAsync(string valor);
        Task CrearAsync(Comentarios comentario);
        Task ActualizarAsync(Comentarios comentario);
        Task EliminarAsync(int id);

        Task<IEnumerable<Comentarios>> ObtenerPorTicketIdAsync(int ticketId);
    }
}
