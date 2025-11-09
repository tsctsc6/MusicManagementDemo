using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Infrastructure.Database.DbConfig.Music;

public class MusicInfoConfiguration : IEntityTypeConfiguration<MusicInfo>
{
    public void Configure(EntityTypeBuilder<MusicInfo> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).IsRequired().HasMaxLength(200).IsUnicode();

        builder.Property(e => e.Artist).IsRequired().HasMaxLength(100).IsUnicode();

        builder.Property(e => e.Album).IsRequired().HasMaxLength(100).IsUnicode();

        builder.Property(e => e.FilePath).IsRequired().HasMaxLength(256).IsUnicode();

        builder
            .Property(e => e.TitleTSV)
            .HasColumnType("TSVECTOR")
            // 指定文本搜索配置
            .HasAnnotation("TsVectorConfig", NpgsqlValues.TsConfigSimple)
            .HasComputedColumnSql(
                $"""to_tsvector('{NpgsqlValues.TsConfigSimple}', "Title")""",
                true
            );
        // 创建 GIN 索引
        builder.HasIndex(e => e.TitleTSV).HasMethod("GIN");

        builder
            .HasIndex(e => new
            {
                e.Title,
                e.Artist,
                e.Album,
            })
            .IsUnique();
        builder.HasIndex(e => e.FilePath).IsUnique();
    }
}
