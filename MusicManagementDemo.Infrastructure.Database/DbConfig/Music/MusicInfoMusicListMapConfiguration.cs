using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Infrastructure.Database.DbConfig.Music;

public class MusicInfoMusicListMapConfiguration : IEntityTypeConfiguration<MusicInfoMusicListMap>
{
    public void Configure(EntityTypeBuilder<MusicInfoMusicListMap> builder)
    {
        builder.HasKey(e => new { e.MusicListId, e.MusicInfoId });

        // 按照 ASCII 排序规则排序
        builder.Property(e => e.SortingOrder).HasMaxLength(128).UseCollation("C");

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

        builder.HasIndex(e => e.SortingOrder).IsUnique();

        // 添加 CHECK 约束强制只允许 ASCII 中的可打印字符（码点 32-126）
        builder.ToTable(t =>
            t.HasCheckConstraint(
                "CK_SortingOrder_PrintableASCII",
                "\"SortingOrder\" ~ '^[\\x20-\\x7E]*$'"
            )
        );
    }
}
