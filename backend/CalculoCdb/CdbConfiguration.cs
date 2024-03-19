using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApi.CalculoCdb;

public class CdbConfiguration : IEntityTypeConfiguration<Cdb>
{
    public void Configure(EntityTypeBuilder<Cdb> builder)
    {
        builder.ToTable("configuracaoCdb");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasDefaultValueSql("(newid())")
            .HasColumnName("id");

        builder.Property(e => e.Cdi)
            .HasColumnName("cdi")
            .HasPrecision(18, 6)
            .IsRequired();

        builder.Property(e => e.TaxaBancaria)
            .HasColumnName("tb")
            .HasPrecision(18, 6)
            .IsRequired();

        builder.Property(e => e.CreatedAt).HasColumnName("createdAt");
        builder.Property(e => e.UpdatedAt).HasColumnName("updatedAt");
    }
}
