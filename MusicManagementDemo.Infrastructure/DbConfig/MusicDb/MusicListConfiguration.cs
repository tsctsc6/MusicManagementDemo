using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Infrastructure.DbConfig.MusicDb;

public class MusicListConfiguration : IEntityTypeConfiguration<MusicList>
{
    public void Configure(EntityTypeBuilder<MusicList> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(100);

        builder.Property(e => e.UserId).HasMaxLength(36);

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
    }
}
