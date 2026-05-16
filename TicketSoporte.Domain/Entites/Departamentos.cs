using System;
using System.Collections.Generic;
using System.Text;

namespace TicketSoporte.Domain.Entites
{
    public class Departamentos
    {
        public int Id { get; set; }
        public string NombreDepartamento { get; set; } = null!;
        public string PrioridadBase { get; set; } =  null!;
        public int? EncargadoId { get; set; }
        public virtual Usuarios Encargado { get; set; } = null!;

        public virtual ICollection<Tickets> Departamento { get; set; } = new List<Tickets>();
    }
}
