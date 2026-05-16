using System.ComponentModel.DataAnnotations;

namespace TicketSoporte.Api.Request
{
    public class TicketsEditarRequest
    {
        [Required(ErrorMessage = "El asunto es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El Asunto no puede exeder los 50 caracteres.")]
        public string Asunto { get; set; } = null!;

        [MaxLength(250, ErrorMessage = "La descripcion no puede exceder los 250 caracteres.")]
        public string? Descripcion { get; set; }


        [Required(ErrorMessage = "El número de serie del equipo es obligatorio.")]
        [MaxLength(40, ErrorMessage = "El Numero de serie no puede exceder los 40 caracteres.")]
        public string NumeroSerieEquipo { get; set; } = null!;

        [Required(ErrorMessage = "Debes asignar un Id de cliente (Es su Id De usuario).")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Debes asignar a que departamento va su equipo.")]
        public int DepartamentoId { get; set; }


        public int? TecnicoAsignadoId { get; set; }
    }
}
