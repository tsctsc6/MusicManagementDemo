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

        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.Name).IsRequired().IsUnicode().HasMaxLength(50);

        builder.HasIndex(x => x.Path).IsUnique();
        builder.Property(x => x.Path).IsRequired().IsUnicode().HasMaxLength(256);
    }
}
