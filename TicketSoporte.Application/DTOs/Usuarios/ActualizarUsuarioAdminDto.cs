namespace TicketSoporte.Application.DTOs.Usuarios
{
    public class ActualizarUsuarioAdminDto
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string? Password { get; set; }
    }
}