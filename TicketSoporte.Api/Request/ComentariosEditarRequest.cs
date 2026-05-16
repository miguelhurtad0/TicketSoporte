using System.ComponentModel.DataAnnotations;

namespace TicketSoporte.Api.Request
{
    public class ComentariosEditarRequest
    {

        [Required(ErrorMessage = "El mensaje no puede estar vacío.")]
        [MaxLength(250, ErrorMessage = "El mensaje no puede exceder los 250 caracteres.")]
        public string Mensaje { get; set; } = null!;

        [MaxLength(250, ErrorMessage = "El mensaje interno no puede exceder los 250 caracteres.")]
        public string? EsInterno { get; set; }
    }
}
