using System;
using System.Collections.Generic;
using System.Text;

namespace TicketSoporte.Domain.Entites
{
    public class Comentarios
    {

        public int Id { get; set; }
        public int TikectId { get; set; }
        public int AutorId { get; set; }
        public string Mensaje { get; set; } = null!;
        public DateOnly FechaCreacion { get; private set; } = DateOnly.FromDateTime(DateTime.Now);
        public string? EsInterno { get; set; }

        public virtual Tickets ticket { get; set; } = null!;
        public virtual Usuarios Autor { get; set; } = null!;
        
    }
}
