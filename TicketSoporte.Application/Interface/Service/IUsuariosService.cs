using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using TicketSoporte.Application.DTOs.Usuarios;

namespace TicketSoporte.Application.Interface.Service
{
    public interface IUsuariosService
    {
        Task<UsuariosDto?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<UsuariosDto>> ObtenerUsuariosAsync(int pagina, int tamano);
        Task<int> ContarAsync();
      
    }
}
