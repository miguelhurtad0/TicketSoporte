using System.ComponentModel.DataAnnotations;

namespace TicketSoporte.Api.Request
{
    public class DepartamentosCrearRequest
    {

        [Required(ErrorMessage = "El nombre del departamento es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El Nombre del Departamento no puede exceder los 20 caracteres.")]
        public string NombreDepartamento { get; set; } = null!;

        [Required(ErrorMessage = "La prioridad base (Alta, Media, Baja) es obligatoria.")]
        [MaxLength(5, ErrorMessage = "El Nombre de Usuario no puede exceder los 5 caracteres.")]
        public string PrioridadBase { get; set; } = null!;

        [Required(ErrorMessage = "Debes asignar un encargado al departamento.")]
        public int EncargadoId { get; set; }
    }
}
