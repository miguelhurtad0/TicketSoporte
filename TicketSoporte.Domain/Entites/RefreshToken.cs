

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSoporte.Domain.Entites
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expiracion { get; set; }
        public int UsuarioId { get; set; }

        public virtual Usuarios Usuario { get; set; } = null!;
    }
}
