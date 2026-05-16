
using TicketSoporte.Domain.Entites;

namespace TicketSoporte.Application.Interface.Repository
{
    public interface IDepartamentosRepository
    {
        
            Task<Departamentos?> ObtenerPorIdAsync(int id);
            Task<IEnumerable<Departamentos>> ObtenerDepartamentosAsync(int numPagina, int cantidad);
            Task<IEnumerable<Departamentos>> BuscarDepartamentosAsync(string valor, int numPagina, int cantidad);
            Task<bool> ExisteNombreAsync(string nombre);
            Task<int> ContarAsync();
            Task<int> ContarBusquedaAsync(string valor);

            Task CrearAsync(Departamentos departamento);
            Task ActualizarAsync(Departamentos departamento);
            
    }
}
