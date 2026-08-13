
using TicketSoporte.Application.DTOs.Tickets;
using TicketSoporte.Domain.Entites;

namespace TicketSoporte.Application.Interface.Repository
{
    public interface ITicketsRepository
    {
       
        Task<Tickets?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Tickets>> ObtenerTicketsAsync(int numPagina, int cantidad);
        Task<IEnumerable<Tickets>> BuscarTicketsAsync(string valor, int numPagina, int cantidad);
        Task<IEnumerable<Tickets>> ObtenerPorDepartamentoAsync(int departamentoId, int pagina, int tamano);
        Task<IEnumerable<Tickets>> ObtenerPorClienteAsync(int clienteId, int pagina, int tamano);
        Task<IEnumerable<Tickets>> ObtenerPorTecnicoAsync(int tecnicoId, int pagina, int tamano);

        Task<int> ContarPorTecnicoAsync(int tecnicoId);
        Task<int> ContarPorClienteAsync(int clienteId);
        Task<int> ContarPorDepartamentoAsync(int departamentoId);
        Task<int> ContarAsync();
        Task<int> ContarBusquedaAsync(string valor);

        Task CrearAsync(Tickets ticket);
        Task ActualizarAsync(Tickets ticket);
        Task EliminarAsync(int id);
        Task CambiarEstadoAsync(int id, string nuevoEstado);
    }
}
