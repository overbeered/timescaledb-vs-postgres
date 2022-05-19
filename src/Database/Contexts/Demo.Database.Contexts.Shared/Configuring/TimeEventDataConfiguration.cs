using Demo.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Database.Contexts.Shared
{
    public class TimeEventDataConfiguration : IEntityTypeConfiguration<TimeEventData>
    {
        public void Configure(EntityTypeBuilder<TimeEventData> builder)
        {
            builder.ToTable("timeeventsdata");

            builder.HasNoKey();

            builder.HasIndex(timeEventData => timeEventData.EventId);

            builder.HasIndex(timeEventData => new { timeEventData.Timestamp, timeEventData.StudentId });

            builder.Property(timeEventData => timeEventData.StudentId)
                .HasColumnName("studentid");

            builder.Property(timeEventData => timeEventData.EventId)
                .HasColumnName("eventid");

            builder.Property(timeEventData => timeEventData.Timestamp)
                .HasColumnName("timestamp")
                .HasColumnType("timestamp with time zone");

            builder.Property(timeEventData => timeEventData.Payload)
                .HasColumnName("payload")
                .HasColumnType("json")
                .IsRequired();
        }
    }
}
