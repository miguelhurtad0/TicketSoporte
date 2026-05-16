
namespace TicketSoporte.Application.DTOs.Comentarios
{
    public class ComentariosDto
    {
        public int Id { get; set; }
        public int TikectId { get; set; } 
        public int AutorId { get; set; }
        public string Mensaje { get; set; } = null!;
        public DateOnly FechaCreacion { get; set; }
        public string? EsInterno { get; set; }
    }
}
