using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Controllers;

namespace WebApplication1.Dados.EntityFramework.Configuration
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuarios>
    {
        public void Configure(EntityTypeBuilder<Usuarios> builder)
        {
            builder.ToTable("Usuarios");
            builder.HasKey(f => f.UsuariosID);

            builder
                .Property(f => f.UsuariosID)
                .UseIdentityColumn()
                .HasColumnName("UsuariosID")
                .HasColumnType("INT");


            builder
                .Property(f => f.Nome)
                .HasColumnName("Nome")
                .HasColumnType("VARCHAR(255)");




            builder
                .Property(f => f.Email)
                .HasColumnName("Email")
                .HasColumnType("VARCHAR(255) UNIQUE");

            builder

                .Property(f => f.Senha)
                .HasColumnName("Senha")
                .HasColumnType("VARCHAR(255)");


            builder
            .Property(f => f.Foto)
            .HasColumnName("Foto")
            .HasColumnType("VARBINARY(MAX)");


        }
    }
}
