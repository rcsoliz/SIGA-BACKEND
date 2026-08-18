using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class CaptadorConfiguration : IEntityTypeConfiguration<Captador>
{
    public void Configure(EntityTypeBuilder<Captador> builder)
    {
        builder.HasMany(c => c.Estancias)
            .WithOne(e => e.Captador)
            .HasForeignKey(e => e.CaptadorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
