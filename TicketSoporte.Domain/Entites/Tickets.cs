using System;
using System.Collections.Generic;
using System.Text;

namespace TicketSoporte.Domain.Entites
{
    public class Tickets
    {
        public int Id { get; set; }
        public string Asunto { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public DateOnly FechaCreacion { get; private set; } = DateOnly.FromDateTime(DateTime.Now);
        public string Estado { get; set; } = "Abierto";
        public int ClienteId { get; set; }
        public int DepartamentoId { get; set; }
        public int? TecnicoAsignadoId  { get; set; }
        public string NumeroSerieEquipo { get; set; } = null!;


        public virtual Usuarios Cliente { get; set; } = null!;
        public virtual Departamentos Departamentos { get; set; } = null!;

        public virtual Usuarios Tecnico { get; set; } = null!;


        public virtual ICollection<Comentarios> Ticket { get; set; } = new List<Comentarios>();




    }
}
