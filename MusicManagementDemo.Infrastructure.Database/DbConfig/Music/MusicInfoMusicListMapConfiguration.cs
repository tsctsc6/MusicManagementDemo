using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Infrastructure.Database.DbConfig.Music;

public class MusicInfoMusicListMapConfiguration : IEntityTypeConfiguration<MusicInfoMusicListMap>
{
    public void Configure(EntityTypeBuilder<MusicInfoMusicListMap> builder)
    {
        builder.HasKey(e => new { e.MusicListId, e.MusicInfoId });

        builder.Property(e => e.SortingOrder).HasMaxLength(128);

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

        builder.HasIndex(e => e.SortingOrder).IsUnique();
    }
}
