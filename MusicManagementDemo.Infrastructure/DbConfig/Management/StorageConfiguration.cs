using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManagementDemo.Domain.Entity.Management;

namespace MusicManagementDemo.Infrastructure.DbConfig.Management;

public class StorageConfiguration : IEntityTypeConfiguration<Storage>
{
    public void Configure(EntityTypeBuilder<Storage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);

        builder.Property(x => x.Path).IsRequired().HasMaxLength(256);
    }
}
