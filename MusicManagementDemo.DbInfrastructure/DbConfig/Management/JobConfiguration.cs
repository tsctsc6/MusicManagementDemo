using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicManagementDemo.Domain.Entity.Management;

namespace MusicManagementDemo.DbInfrastructure.DbConfig.Management;

public class JobConfiguration(JsonSerializerOptions jsonSerializerOptions)
    : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(j => j.Id);
        builder.Property(j => j.Id).ValueGeneratedOnAdd();

        builder.Property(j => j.Type).IsRequired().HasDefaultValue(JobType.Undefined);

        builder
            .Property(j => j.JobArgs)
            .HasColumnType("jsonb")
            .HasConversion(
                v => v.ToJsonString(jsonSerializerOptions),
                v => JsonNode.Parse(v, null, default) ?? "{}"
            )
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(j => j.Status).IsRequired().HasDefaultValue(JobStatus.Undefined);

        builder.Property(j => j.Description).IsRequired().IsUnicode().HasMaxLength(500);

        builder.Property(j => j.Description).IsRequired().IsUnicode().HasMaxLength(500);

        builder.Property(j => j.Success).IsRequired().HasDefaultValue(false);

        builder.Property(j => j.CreatedAt).IsRequired().HasDefaultValueSql("now()");

        builder.Property(j => j.CompletedAt).IsRequired(false);
    }
}
