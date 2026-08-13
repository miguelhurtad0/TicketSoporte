

using Microsoft.EntityFrameworkCore;
using TicketSoporte.Application.Interface.Repository;
using TicketSoporte.Domain.Entites;
using TicketSoporte.Infrastructure.Data;

namespace TicketSoporte.Infrastructure.Repository
{
    public class ComentariosRepository : IComentariosRepository
    {
        private readonly ApplicationDbContext _context;

        public ComentariosRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarAsync(Comentarios comentario)
        {
            _context.Comentarios.Update(comentario);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Comentarios>> BuscarComentariosAsync(string valor, int numPagina, int cantidad)
        {
            var query = _context.Comentarios
                .Include(c => c.Autor)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(valor))
            {
                var busqueda = valor.Trim().ToLower();
                // Buscamos dentro del texto del mensaje
                query = query.Where(c => c.Mensaje.ToLower().Contains(busqueda));
            }

            return await query
                .OrderByDescending(c => c.FechaCreacion)
                .Skip((numPagina - 1) * cantidad)
                .Take(cantidad)
                .ToListAsync();
        }

        public async Task<int> ContarAsync()
        {
           return await _context.Comentarios.CountAsync();
        }

        public async Task<int> ContarBusquedaAsync(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return await _context.Comentarios.CountAsync();

            var busqueda = valor.Trim().ToLower();
            return await _context.Comentarios
                .CountAsync(c => c.Mensaje.ToLower().Contains(busqueda));
        }

        public async Task CrearAsync(Comentarios comentario)
        {
            await _context.Comentarios.AddAsync(comentario);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);
            if (comentario != null)
            {
                _context.Comentarios.Remove(comentario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Comentarios>> ObtenerComentariosAsync(int numPagina, int cantidad)
        {
            return await _context.Comentarios
                .Include(c => c.Autor)
                .OrderByDescending(c => c.FechaCreacion)
                .Skip((numPagina - 1) * cantidad)
                .Take(cantidad)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Comentarios?> ObtenerPorIdAsync(int id)
        {
            return await _context.Comentarios
                .Include(c => c.Autor) 
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Comentarios>> ObtenerPorTicketIdAsync(int ticketId)
        {
            return await _context.Comentarios
                .Include(c => c.Autor)
                .Where(c => c.TikectId == ticketId)
                .OrderBy(c => c.FechaCreacion) 
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Comentarios>> ObtenerPorTicketIdAsync(int ticketId, bool ocultarInternos)
        {
            var query = _context.Comentarios
                .Include(c => c.Autor)
                .Where(c => c.TikectId == ticketId); // Usando tu propiedad actual

            // Filtro de seguridad: Si es verdadero, omitimos los comentarios internos
            if (ocultarInternos)
            {
                // NOTA: Asegúrate de que tu entidad tenga la propiedad 'EsInterno' (o el nombre que le hayas puesto)
                query = query.Where(c => c.EsInterno == "false" || c.EsInterno == null);
            }

            return await query
                .OrderBy(c => c.FechaCreacion)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
