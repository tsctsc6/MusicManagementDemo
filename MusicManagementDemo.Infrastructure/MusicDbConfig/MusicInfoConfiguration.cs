using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Infrastructure.MusicDbConfig;

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
            .HasAnnotation("TsVectorConfig", "english")
            .HasComputedColumnSql("""to_tsvector('english', "Title")""", true);
        // 创建 GIN 索引
        builder.HasIndex(e => e.TitleTSV).HasMethod("GIN");
    }
}
