using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WebApplication1.Controllers;
using WebApplication1.Dados.EntityFramework.Configuration;

namespace WebApplication1.Dados.EntityFramework
{
    public class Contexto : DbContext
    {
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Grupos> Grupos { get; set; }
        public DbSet<ParticipantesGrupo> ParticipantesGrupo { get; set; }

        public DbSet<Convite> Convites { get; set; }

        public Contexto(DbContextOptions<Contexto> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuarios>(entity =>
            {
                entity.HasKey(e => e.UsuariosID);
                entity.Property(e => e.UsuariosID).UseIdentityColumn();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.ToTable("Usuarios");
            });

            modelBuilder.Entity<Grupos>(entity =>
            {
                entity.HasKey(e => e.GruposID);
                entity.ToTable("Grupos");
            });
            modelBuilder.Entity<Convite>(entity =>
            {
                entity.ToTable("Convites");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ConviteID).IsRequired();
                entity.Property(e => e.GrupoID).IsRequired();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(255);
                entity.Property(e => e.DataExpiracao).IsRequired();
                entity.Property(e => e.Usado).IsRequired().HasDefaultValue(false);
                entity.HasOne<Grupos>().WithMany().HasForeignKey(e => e.GrupoID)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_Convites_Grupos");
            });
            modelBuilder.Entity<ParticipantesGrupo>()
            .HasKey(pg => new { pg.ID_Grupo, pg.ID_Participante });

            modelBuilder.Entity<ParticipantesGrupo>()
                .HasOne(pg => pg.Grupos)
                .WithMany(g => g.ParticipantesGrupo)
                .HasForeignKey(pg => pg.ID_Grupo);

            modelBuilder.Entity<ParticipantesGrupo>()
                .HasOne(pg => pg.Usuarios)
                .WithMany(u => u.Grupos)
                .HasForeignKey(pg => pg.ID_Participante);


        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data source=201.62.57.93,1434; 
                                           Database=BD038216; 
                                           User ID=RA038216; 
                                           Password=038216; 
                                           TrustServerCertificate=True");
        }
    }
}