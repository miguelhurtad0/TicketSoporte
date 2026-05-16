using Microsoft.EntityFrameworkCore;
using TicketSoporte.Domain.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace TicketSoporte.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuarios,IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Departamentos> Departamentos { get; set; }
        public DbSet<Tickets> Tickets { get; set; }
        public DbSet<Comentarios> Comentarios { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Departamentos>(static entity =>
            {
                entity.HasKey(d => d.Id);

                entity.Property(d => d.Id)
                .ValueGeneratedOnAdd();

                entity.Property(d => d.NombreDepartamento)
                .IsRequired()
                .HasMaxLength(20);

                entity.Property(d => d.PrioridadBase)
               .HasMaxLength(5);

                entity.ToTable(table =>
                {
                    table.HasCheckConstraint("CK_Departamentos_PrioridadBase", "\"PrioridadBase\" IN('Alta','Media', 'Baja')");
                });

                //Relacion con la entidad user
                entity.HasOne(d => d.Encargado)
                .WithMany(d => d.Departamentos)
                .HasForeignKey(p => p.EncargadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();





            });

            builder.Entity<Tickets>(static entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Id)
                .ValueGeneratedOnAdd();

                entity.Property(t => t.Asunto)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(t => t.Descripcion)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(t => t.FechaCreacion)
                  .IsRequired()
                  .HasColumnType("date");

                entity.Property(t => t.Estado)
               .HasMaxLength(9);

                entity.Property(t => t.TecnicoAsignadoId)
                .IsRequired(false);


                entity.Property(t => t.NumeroSerieEquipo)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.ToTable(table =>
                {
                    table.HasCheckConstraint("CK_Tickets_Estado", "\"Estado\" IN('Abierto','Procesado', 'Cerrado')");
                });


                //Relacion con la entidad user
                entity.HasOne(t => t.Cliente)
                .WithMany(t => t.Clientes)
                .HasForeignKey(t => t.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();

                //Relacion con la entidad departamento
                entity.HasOne(t => t.Departamentos)
                .WithMany(t => t.Departamento)
                .HasForeignKey(t => t.DepartamentoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();

                //Relacion con la entidad user
                entity.HasOne(t => t.Tecnico)
                .WithMany(t => t.Tecnicos)
                .HasForeignKey(t => t.TecnicoAsignadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired(false);


            });

            builder.Entity<Comentarios>(static entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Id)
                .ValueGeneratedOnAdd();

                entity.Property(c => c.Mensaje)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(c => c.FechaCreacion)
                 .IsRequired()
                 .HasColumnType("date");

                entity.Property(c => c.EsInterno)
                    .IsRequired(false)
                    .HasMaxLength(250);

                //Relacion con la entidad user
                entity.HasOne(c => c.ticket)
                .WithMany(c => c.Ticket)
                .HasForeignKey(c => c.TikectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();

                //Relacion con la entidad user
                entity.HasOne(c => c.Autor)
                .WithMany(c => c.Autors)
                .HasForeignKey(c => c.AutorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired();

            });

            builder.Entity<RefreshToken>(entity =>
            {
                  entity.HasOne(d => d.Usuario)          
                        .WithMany(p => p.RefreshTokens)    
                        .HasForeignKey(d => d.UsuarioId)   
                        .OnDelete(DeleteBehavior.Cascade);
               
            });
        
        } 

    }
}
