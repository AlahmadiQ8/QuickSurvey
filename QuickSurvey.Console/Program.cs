using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuickSurvey.Core.SessionAggregate;
using QuickSurvey.Infrastructure;
using QuickSurvey.Infrastructure.Repositories;

namespace QuickSurvey.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //var context = CreateDbContext();
            //context.Database.EnsureDeleted();
            //context.Database.Migrate();
            //var repository = new SessionRepository(context);

            //var session = new Session("Friday Night Plans");
            //session.AddChoices(new []{"Chelsea", "HK", "Bushwick"});
            //session.AddParticipant("Mohammad");
            //session.AddParticipant("Mark");
            //session.AddParticipant("Will");
            //repository.Add(session);
            //await repository.UnitOfWork.SaveChangesAsync();
        }

        public static SurveyContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SurveyContext>()
                .UseSqlServer(SurveyContext.ConnectionString);
            var context = new SurveyContext(optionsBuilder.Options);
            return context;
        }
    }
}
