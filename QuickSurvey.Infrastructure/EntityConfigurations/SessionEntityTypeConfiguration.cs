using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickSurvey.Core.SessionAggregate;

namespace QuickSurvey.Infrastructure.EntityConfigurations
{
    class SessionEntityTypeConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(p => p.Id);

            // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implementation-entity-framework-core#the-hilo-algorithm-in-ef-core
            builder.Property(p => p.Id).UseHiLo("sessionseq");

            builder.OwnsMany(
                s => s.Participants,
                p =>
                {
                    p.HasKey(p => p.Id);
                    p.WithOwner().HasForeignKey(p => p.SessionId);
                    p.HasIndex(p => new {p.Username, SessiontId = p.SessionId}).IsUnique();
                }
            );

            // https://github.com/dotnet/efcore/issues/17471#issuecomment-526330450
            var comparer = new ValueComparer<IReadOnlyCollection<string>>(
                (c1, c2) => c1.Equals(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => (IReadOnlyCollection<string>)c.ToHashSet()
            );

            builder.OwnsMany(
                s => s.Choices,
                c =>
                {
                    c.WithOwner().HasForeignKey("SessionId");
                    c.HasKey(cc => cc.Id);
                    c.Property(cc => cc.Voters)
                        .HasField("_voters")
                        .HasConversion(
                            l => string.Join(",", l),
                            s => s.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList())
                        .Metadata.SetValueComparer(comparer);
                    c.HasIndex(p => new { p.Text, SessiontId = p.SessionId }).IsUnique();
                }
            );
        }
    }
}
