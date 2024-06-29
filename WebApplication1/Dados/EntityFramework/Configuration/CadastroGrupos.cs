using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Controllers;

namespace WebApplication1.Dados.EntityFramework.Configuration
{
    public class GruposConfiguration : IEntityTypeConfiguration<Grupos>
    {
        public void Configure(EntityTypeBuilder<Grupos> builder)
        {
            builder.ToTable("Grupos");
            builder.HasKey(f => f.GruposID);

            builder
                .Property(f => f.GruposID)
                .UseIdentityColumn()
                .HasColumnName("GruposID")
                .HasColumnType("INT");


            builder
                .Property(f => f.NomeGrupo)
                .HasColumnName("NomeGrupo")
                .HasColumnType("VARCHAR(255)");




            builder
                .Property(f => f.ParticipantesMax)
                .HasColumnName("ParticipantesMax")
                .HasColumnType("INT");

            builder

                .Property(f => f.Valor)
                .HasColumnName("Valor")
                .HasColumnType("DECIMAL(10, 2)");


            builder
                .Property(f => f.DataRevelacao)
                .HasColumnName("DataRevelacao")
                .HasColumnType("DATE");


            builder
                .Property(f => f.Descricao)
                .HasColumnName("Descricao")
                .HasColumnType("VARCHAR(255)");


            builder

                .Property(f => f.SorteioRealizado)
                .HasColumnName("SorteioRealizado")
                .HasColumnType("BIT");


            builder
                .Property(f => f.Foto)
                .HasColumnName("Foto")
                .HasColumnType("VARBINARY(MAX)");

        }
    }
}
