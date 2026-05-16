using System;
using System.Collections.Generic;
using System.Text;
using TicketSoporte.Domain.Entites;

namespace TicketSoporte.Application.Interface.Repository
{
    public interface IRefreshTokenRepository
    {
        Task GuardarAsync(RefreshToken token);
        Task<RefreshToken?> ObtenerAsync(string token);
        Task ActualizarAsync(RefreshToken token);
    }
}
