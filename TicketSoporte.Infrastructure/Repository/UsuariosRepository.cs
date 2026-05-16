
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
    }
}
