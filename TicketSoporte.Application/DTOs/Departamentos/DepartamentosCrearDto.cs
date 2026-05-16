

using System.ComponentModel.DataAnnotations;

namespace TicketSoporte.Application.DTOs.Departamentos
{
    public class DepartamentosCrearDto
    {
        public string NombreDepartamento { get; set; } = null!;
        public string PrioridadBase { get; set; } = null!;
        public int EncargadoId { get; set; }
    }
}
