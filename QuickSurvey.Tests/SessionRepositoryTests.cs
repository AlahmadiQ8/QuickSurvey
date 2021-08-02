using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using QuickSurvey.Core.SessionAggregate;
using QuickSurvey.Infrastructure;
using QuickSurvey.Infrastructure.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace QuickSurvey.Tests
{
    public class SessionRepositoryTests : IDisposable
    {
        private readonly SessionRepository _repository;

        public SessionRepositoryTests(ITestOutputHelper output)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SurveyContext>()
                .UseSqlServer(SurveyContext.ConnectionString)
                .LogTo(output.WriteLine);
            var context = new SurveyContext(optionsBuilder.Options);
            context.Database.EnsureDeleted();
            context.Database.Migrate();
            _repository = new SessionRepository(context);
        }

        [Fact]
        public async Task SessionRepository_CreatesSession_Succeeds()
        {
            // Arrange
            var session = new Session("Friday Night");
            Console.WriteLine("Hello World");


            // Act
            _repository.Add(session);
            var success = await _repository.UnitOfWork.SaveEntitiesAsync();
            var id = session.Id;

            // Assert
            success.Should().BeTrue();
            var sessionQueried = await _repository.GetAsync(id);
            sessionQueried.Should().Be(session);
        }

        [Fact]
        public async Task SessionRepository_AddParticipants_Succeeds()
        {
            // Arrange
            var session = new Session("Friday Night");
            _repository.Add(session);
            await _repository.UnitOfWork.SaveEntitiesAsync();
            session = await _repository.GetAsync(session.Id);

            // Act
            session.AddParticipant("Mark");
            session.AddParticipant("Mohammad");
            _repository.Update(session);
            await _repository.UnitOfWork.SaveEntitiesAsync();

            // Assert
            session = await _repository.GetAsync(session.Id);
            session.Participants.ToList().Find(p => p.Username == "Mark").Should().NotBeNull();
            session.Participants.ToList().Find(p => p.Username == "Mohammad").Should().NotBeNull();
        }

        [Fact]
        public async Task SessionRepository_AddChoices_Succeeds()
        {
            // Arrange
            var session = new Session("Friday Night");
            _repository.Add(session);
            await _repository.UnitOfWork.SaveEntitiesAsync();
            session = await _repository.GetAsync(session.Id);

            // Act
            session.AddChoices(new[] { "one", "two", "three" });
            _repository.Update(session);
            await _repository.UnitOfWork.SaveEntitiesAsync();

            // Assert
            session = await _repository.GetAsync(session.Id);
            session.Choices.Select(c => c.Text).Should().BeEquivalentTo(new[] { "one", "two", "three" });
        }

        [Fact]
        public async Task SessionRepository_SetVotes_Succeeds()
        {
            // Arrange
            var session = new Session("Friday Night");
            _repository.Add(session);
            await _repository.UnitOfWork.SaveEntitiesAsync();
            session = await _repository.GetAsync(session.Id);
            session.AddParticipant("Mark");
            session.AddParticipant("Mohammad");
            session.AddChoices(new[] { "one", "two", "three" });
            _repository.Update(session);
            await _repository.UnitOfWork.SaveEntitiesAsync();
            var choiceId = session.Choices.Single(c => c.Text == "one").Id;

            // Act
            session.SetVote("Mark", choiceId);
            _repository.Update(session);
            await _repository.UnitOfWork.SaveEntitiesAsync();

            // Assert
            session = await _repository.GetAsync(session.Id);
            var choice = session.Choices.Single(c => c.Id == choiceId);
            choice.Voters.SingleOrDefault(v => v == "Mark").Should().NotBeNull();
        }

        public void Dispose()
        {
            _repository.Dispose();

        }
    }
}
