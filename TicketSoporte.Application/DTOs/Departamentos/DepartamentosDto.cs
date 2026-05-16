using System;
namespace TicketSoporte.Application.DTOs.Departamentos
{
    public class DepartamentosDto
    {
        public int Id { get; set; }
        public string NombreDepartamento { get; set; } = null!;
        public string PrioridadBase { get; set; } = null!;
        public int EncargadoId { get; set; }
    }
}
