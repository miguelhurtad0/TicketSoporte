using Microsoft.EntityFrameworkCore;
using TicketSoporte.Application.Interface.Repository;
using TicketSoporte.Domain.Entites;
using TicketSoporte.Infrastructure.Data;

namespace TicketSoporte.Infrastructure.Repository
{
    public class UsuariosRepository : IUsuariosRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuariosRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> ContarAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<Usuarios?> ObtenerPorIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<Usuarios?> ObtenerPorIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<Usuarios>> ObtenerUsuariosAsync(int pagina, int tamano)
        {
            return await _context.Users
               .AsNoTracking()
               .OrderBy(u => u.UserName)
               .Skip((pagina - 1) * tamano)
               .Take(tamano)
               .ToListAsync();
        }

        // ¡NUEVO! Cuenta cuántos usuarios coinciden con la búsqueda
        public async Task<int> ContarBusquedaAsync(string nombre)
        {
            return await _context.Users
                .Where(u => (u.NombreCompleto != null && u.NombreCompleto.ToLower().Contains(nombre.ToLower())) ||
                             u.UserName!.ToLower().Contains(nombre.ToLower()))
                .CountAsync();
        }

        // ¡NUEVO! Trae los usuarios filtrados y paginados
        public async Task<IEnumerable<Usuarios>> BuscarPorNombreAsync(string nombre, int pagina, int tamano)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => (u.NombreCompleto != null && u.NombreCompleto.ToLower().Contains(nombre.ToLower())) ||
                             u.UserName!.ToLower().Contains(nombre.ToLower()))
                .OrderBy(u => u.NombreCompleto) // Ordenamos por nombre
                .Skip((pagina - 1) * tamano)
                .Take(tamano)
                .ToListAsync();
        }
    }
}
