
namespace TicketSoporte.Application.DTOs.Tickets
{
    public class TicketsDto
    {
        public int Id { get; set; }
        public string Asunto { get; set; } = null!;
        public string? Descripcion { get; set; }
        public DateOnly FechaCreacion { get; set; }
        public string Estado { get; set; } = null!;
        public string NumeroSerieEquipo { get; set; } = null!;
        public int ClienteId { get; set; }
        public int DepartamentoId { get; set; }
        public int? TecnicoAsignadoId { get; set; }
    }
}
