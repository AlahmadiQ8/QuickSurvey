using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuickSurvey.Core.SeedWork;
using QuickSurvey.Core.SessionAggregate;
using QuickSurvey.Infrastructure.EntityConfigurations;

namespace QuickSurvey.Infrastructure
{
    public class SurveyContext : DbContext, IUnitOfWork
    {
        public const string ConnectionString = "Data Source=(localdb)\\DEVELOPMENT;Database=quicksurvey;Connect Timeout=30";
        
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Choice> Choices { get; set; }

        public SurveyContext(DbContextOptions<SurveyContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SessionEntityTypeConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
