using CursoEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CursoEFCore.Data.Configurations
{
    public class PedidoItemConfiguration : IEntityTypeConfiguration<PedidoItem>
    {
        public void Configure(EntityTypeBuilder<PedidoItem> builder)
        {
            builder.ToTable("PedidoItens");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Quantidade).HasDefaultValue(1).IsRequired();
            builder.Property(c => c.Valor).IsRequired();
            builder.Property(c => c.Desconto).IsRequired();         
        }
    }
}