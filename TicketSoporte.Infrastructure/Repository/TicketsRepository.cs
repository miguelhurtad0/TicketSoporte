

using Microsoft.EntityFrameworkCore;
using TicketSoporte.Application.DTOs.Tickets;
using TicketSoporte.Application.Interface.Repository;
using TicketSoporte.Domain.Entites;
using TicketSoporte.Infrastructure.Data;

namespace TicketSoporte.Infrastructure.Repository
{
    public class TicketsRepository : ITicketsRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarAsync(Tickets ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Tickets>> BuscarTicketsAsync(string valor, int numPagina, int cantidad)
        {
            var query = _context.Tickets
                .Include(t => t.Cliente)
                .Include(t => t.Departamentos)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(valor))
            {
                var busqueda = valor.Trim().ToLower();
                query = query.Where(t =>
                    t.Asunto.ToLower().Contains(busqueda) ||
                    t.Estado.ToLower().Contains(busqueda) ||
                    t.Departamentos != null && t.Departamentos.NombreDepartamento.ToLower().Contains(busqueda)||
                    t.NumeroSerieEquipo.ToLower().Contains(busqueda));
            }

            return await query
                .OrderByDescending(t => t.FechaCreacion)
                .Skip((numPagina - 1) * cantidad)
                .Take(cantidad)
                .ToListAsync();
        }

        public async Task CambiarEstadoAsync(int id, string nuevoEstado)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                ticket.Estado = nuevoEstado;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> ContarAsync()
        {
            return await _context.Tickets.CountAsync();
        }

        public async Task<int> ContarBusquedaAsync(string valor)
        {
            var busqueda = valor.Trim().ToLower();
            return await _context.Tickets
                .CountAsync(t => t.Asunto.ToLower().Contains(busqueda) ||
                                 t.Estado.ToLower().Contains(busqueda));
        }

        public async Task<int> ContarPorClienteAsync(int clienteId)
        {
            return await _context.Tickets.CountAsync(t => t.ClienteId == clienteId);
        }

        public async Task<int> ContarPorDepartamentoAsync(int departamentoId)
        {
            return await _context.Tickets.CountAsync(t => t.DepartamentoId == departamentoId);
        }

        public async Task<int> ContarPorTecnicoAsync(int tecnicoId)
        {
            return await _context.Tickets.CountAsync(t => t.TecnicoAsignadoId == tecnicoId);
        }

        public async Task CrearAsync(Tickets ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Tickets>> ObtenerPorClienteAsync(int clienteId, int pagina, int tamano)
        {
            return await _context.Tickets
             .Where(t => t.ClienteId == clienteId)
             .OrderByDescending(t => t.FechaCreacion)
             .Skip((pagina - 1) * tamano)
             .Take(tamano)
             .ToListAsync();
        }

        public async Task<IEnumerable<Tickets>> ObtenerPorDepartamentoAsync(int departamentoId, int pagina, int tamano)
        {
          return await _context.Tickets
              .Where(t => t.DepartamentoId == departamentoId)
              .OrderByDescending(t => t.FechaCreacion)
              .Skip((pagina - 1) * tamano)
              .Take(tamano)
              .ToListAsync();
        }

        public async Task<Tickets?> ObtenerPorIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.Cliente)
                .Include(t => t.Departamentos)
                .Include(t => t.Tecnico)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Tickets>> ObtenerPorTecnicoAsync(int tecnicoId, int pagina, int tamano)
        {
            return await _context.Tickets
                .Include(t => t.Cliente)
                .Include(t => t.Departamentos)
                .Where(t => t.TecnicoAsignadoId == tecnicoId) // Filtramos por el técnico
                .OrderByDescending(t => t.FechaCreacion)
                .Skip((pagina - 1) * tamano)
                .Take(tamano)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Tickets>> ObtenerTicketsAsync(int numPagina, int cantidad)
        {
            return await _context.Tickets
                .Include(t => t.Cliente)
                .Include(t => t.Departamentos)
                .OrderByDescending(t => t.FechaCreacion) // Los más nuevos primero
                .Skip((numPagina - 1) * cantidad)
                .Take(cantidad)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
