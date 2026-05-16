

using System.ComponentModel.DataAnnotations;

namespace TicketSoporte.Application.DTOs.Tickets
{
    public class TicketsEditarDto
    {
        public string Asunto { get; set; } = null!;
        public string? Descripcion { get; set; }
  
        public string NumeroSerieEquipo { get; set; } = null!;
        public int ClienteId { get; set; }
        public int DepartamentoId { get; set; }
        public int? TecnicoAsignadoId { get; set; }
    }
}
