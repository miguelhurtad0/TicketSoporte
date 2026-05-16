

using System.ComponentModel.DataAnnotations;

namespace TicketSoporte.Application.DTOs.Comentarios
{
    public class ComentariosEditarDto
    {
        public string Mensaje { get; set; } = null!;
        public string? EsInterno { get; set; }
    }
}
