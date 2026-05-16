

using System.ComponentModel.DataAnnotations;

namespace TicketSoporte.Application.DTOs.Comentarios
{
    public class ComentariosCrearDto
    {
       
        public int TikectId { get; set; }
        public int AutorId { get; set; }
        public string Mensaje { get; set; } = null!;
        public string? EsInterno { get; set; }
    }
}
