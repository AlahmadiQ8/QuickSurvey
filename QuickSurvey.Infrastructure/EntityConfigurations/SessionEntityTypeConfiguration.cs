using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickSurvey.Core.SessionAggregate;

namespace QuickSurvey.Infrastructure.EntityConfigurations
{
    class SessionEntityTypeConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable("sessions", SurveyContext.DefaultSchema);

            builder.HasKey(p => p.Id);

            // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implementation-entity-framework-core#the-hilo-algorithm-in-ef-core
            //builder.Property(p => p.Id).UseHiLo("sessionseq");

            builder.OwnsMany(
                s => s.Participants,
                p =>
                {
                    p.HasKey(p => p.Id);
                    p.WithOwner().HasForeignKey(p => p.SessionId);
                    p.HasIndex(p => new {p.Username, SessiontId = p.SessionId}).IsUnique();
                }
            );

            builder.OwnsMany(
                s => s.Choices,
                c =>
                {
                    c.HasKey(c => c.Id);
                    c.Property(c => c.Voters)
                        .HasField("_voters")
                        .HasConversion(
                            l => string.Join(",", l),
                            s => s.Split(",", StringSplitOptions.None));
                    c.WithOwner().HasForeignKey(c => c.SessionId);
                    c.HasIndex(p => new { p.Text, SessiontId = p.SessionId }).IsUnique();
                }
            );
        }
    }
}
