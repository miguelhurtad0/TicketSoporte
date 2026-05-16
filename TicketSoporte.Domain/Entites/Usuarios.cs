using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace TicketSoporte.Domain.Entites
{
    public class Usuarios : IdentityUser<int>
    {

        public virtual ICollection<Departamentos> Departamentos { get; set; } = new List<Departamentos>();
        public virtual ICollection<Tickets> Clientes { get; set; } = new List<Tickets>();

        public virtual ICollection<Tickets> Tecnicos { get; set; } = new List<Tickets>();
        public virtual ICollection<Comentarios> Autors { get; set; } = new List<Comentarios>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    }
}
