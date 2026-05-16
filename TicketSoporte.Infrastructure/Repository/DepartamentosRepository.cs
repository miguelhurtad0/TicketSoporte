

using Microsoft.EntityFrameworkCore;
using TicketSoporte.Application.Interface.Repository;
using TicketSoporte.Domain.Entites;
using TicketSoporte.Infrastructure.Data;

namespace TicketSoporte.Infrastructure.Repository
{
    public class DepartamentosRepository : IDepartamentosRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartamentosRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task ActualizarAsync(Departamentos departamento)
        {
            _context.Departamentos.Update(departamento);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Departamentos>> BuscarDepartamentosAsync(string valor, int numPagina, int cantidad)
        {
            var query = _context.Departamentos
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(valor))
            {
                var busqueda = valor.Trim().ToLower();
                // Filtramos por Nombre o por Prioridad
                query = query.Where(d =>
                    d.NombreDepartamento.ToLower().Contains(busqueda) ||
                    d.PrioridadBase.ToLower().Contains(busqueda));
            }

            return await query
                .OrderBy(d => d.NombreDepartamento)
                .Skip((numPagina - 1) * cantidad)
                .Take(cantidad)
                .ToListAsync();
        }

        public async Task<int> ContarAsync()
        {
            return await _context.Departamentos.CountAsync();
        }

        public async Task<int> ContarBusquedaAsync(string valor)
        {
            var busqueda = valor.Trim().ToLower();
            return await _context.Departamentos
                .CountAsync(d => d.NombreDepartamento.ToLower().Contains(busqueda) ||
                                 d.PrioridadBase.ToLower().Contains(busqueda));
        }

        public async Task CrearAsync(Departamentos departamento)
        {
            await _context.Departamentos.AddAsync(departamento);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteNombreAsync(string nombre)
        {
            return await _context.Departamentos
                .AnyAsync(d => d.NombreDepartamento.ToLower() == nombre.ToLower());
        }

        public async Task<IEnumerable<Departamentos>> ObtenerDepartamentosAsync(int numPagina, int cantidad)
        {
            return await _context.Departamentos
                .AsNoTracking()
                .OrderBy(d => d.NombreDepartamento)
                .Skip((numPagina - 1) * cantidad)
                .Take(cantidad)
                .ToListAsync();
        }

        public async Task<Departamentos?> ObtenerPorIdAsync(int id)
        {
            return await _context.Departamentos
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
