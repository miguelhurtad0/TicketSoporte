

using TicketSoporte.Domain.Entites;

namespace TicketSoporte.Application.Interface.Repository
{
    public interface IUsuariosRepository
    {
        Task<Usuarios?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Usuarios>> ObtenerUsuariosAsync(int pagina, int tamano);
        Task<int> ContarAsync();
    }
}
