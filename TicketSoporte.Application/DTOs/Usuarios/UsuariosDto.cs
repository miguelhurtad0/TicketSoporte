

namespace TicketSoporte.Application.DTOs.Usuarios
{
    public class UsuariosDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Rol { get; set; } = null!; 
    }
}
