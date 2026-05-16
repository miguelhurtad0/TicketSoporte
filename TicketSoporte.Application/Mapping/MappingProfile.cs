using AutoMapper;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using TicketSoporte.Application.DTOs.Comentarios;
using TicketSoporte.Application.DTOs.Departamentos;
using TicketSoporte.Application.DTOs.Tickets;
using TicketSoporte.Application.DTOs.Usuarios;
using TicketSoporte.Domain.Entites;

namespace TicketSoporte.Application.Mapping
{
    public class MappingProfile : Profile 
    {
        public MappingProfile()
        {
            #region Mapeo de Comentarios
            CreateMap<ComentariosCrearDto, Comentarios>();
            CreateMap<ComentariosEditarDto, Comentarios>();
            CreateMap<Comentarios, ComentariosDto>().ReverseMap();
            #endregion

            #region Mapeo de Departamentos
            CreateMap<DepartamentosCrearDto, Departamentos>();
            CreateMap<DepartamentosEditarDto, Departamentos>();
            CreateMap<Departamentos, DepartamentosDto>().ReverseMap();
            #endregion

            #region Mapeo de Tickets
            CreateMap<TicketsCrearDto, Tickets>();
            CreateMap<TicketsEditarDto, Tickets>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Tickets, TicketsDto>().ReverseMap();
            #endregion

            #region Mapeo de Usuario

            CreateMap<Usuarios, UsuariosDto>()
                .ForMember(dest => dest.Rol, opt => opt.Ignore());

            CreateMap<UsuariosRegistroDto, Usuarios>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            #endregion
        }

    }
}
