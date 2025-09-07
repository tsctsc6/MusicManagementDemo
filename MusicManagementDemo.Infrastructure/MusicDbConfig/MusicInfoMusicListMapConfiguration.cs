using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Infrastructure.MusicDbConfig;

public class MusicInfoMusicListMapConfiguration : IEntityTypeConfiguration<MusicInfoMusicListMap>
{
    public void Configure(EntityTypeBuilder<MusicInfoMusicListMap> builder)
    {
        builder.HasKey(e => new { e.MusicListId, e.MusicInfoId });

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
    }
}
