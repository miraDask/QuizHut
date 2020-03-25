namespace QuizHut.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data;
    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Models;
    using QuizHut.Data.Repositories;
    using QuizHut.Services.Mapping;
    using QuizHut.Services.Results;
    using QuizHut.Web.ViewModels.Results;
    using Xunit;

    public class ResultsServiceTests
    {
        private readonly ApplicationDbContext dbContext;
        private readonly EfDeletableEntityRepository<Result> resultRepository;
        private readonly EfDeletableEntityRepository<Event> eventRepository;
        private readonly ResultsService service;

        public ResultsServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options;
            this.dbContext = new ApplicationDbContext(options);
            this.resultRepository = new EfDeletableEntityRepository<Result>(this.dbContext);
            this.eventRepository = new EfDeletableEntityRepository<Event>(this.dbContext);
            this.service = new ResultsService(this.resultRepository, this.eventRepository);

            AutoMapperConfig.RegisterMappings(typeof(ScoreViewModel).GetTypeInfo().Assembly);
            AutoMapperConfig.RegisterMappings(typeof(ResultViewModel).GetTypeInfo().Assembly);
        }

        [Fact]
        public async Task GetAllByStudentIdAsyncShouldReturnCorrectModelCollection()
        {
            var studentId = Guid.NewGuid().ToString();
            var activationDate = DateTime.UtcNow;
            var firstEventInfo = await this.CreateEventAsync("First event", activationDate);
            var firstEventId = firstEventInfo[0];
            var secondEventInfo = await this.CreateEventAsync("Second event", activationDate);
            var secondEventId = secondEventInfo[0];

            await this.CreateResultAsync(studentId, 2, 10, firstEventId);
            await this.CreateResultAsync(studentId, 15, 15, secondEventId);

            var firstModel = new ScoreViewModel()
            {
                EventId = firstEventId,
                Score = "2/10",
            };

            var secondModel = new ScoreViewModel()
            {
                EventId = secondEventId,
                Score = "15/15",
            };

            var resultModelCollection = await this.service.GetAllByStudentIdAsync<ScoreViewModel>(studentId);

            Assert.Equal(firstModel.EventId, resultModelCollection.Last().EventId);
            Assert.Equal(firstModel.Score, resultModelCollection.Last().Score);
            Assert.Equal(secondModel.EventId, resultModelCollection.First().EventId);
            Assert.Equal(secondModel.Score, resultModelCollection.First().Score);
            Assert.Equal(2, resultModelCollection.Count());
            Assert.IsAssignableFrom<IEnumerable<ScoreViewModel>>(resultModelCollection);
        }

        [Fact]
        public async Task GetAllResultsByEventIdAsyncShouldReturnCorrectModelCollection()
        {
            var firstStudentId = await this.CreateStudentAsync();
            var secondStudentId = await this.CreateStudentAsync();

            var activationDate = DateTime.UtcNow;
            var eventInfo = await this.CreateEventAsync("First event", activationDate);
            var eventId = eventInfo[0];
            var groupName = await this.AssignStudentsToGroupAsync(new string[] { firstStudentId, secondStudentId });

            await this.CreateResultAsync(firstStudentId, 2, 10, eventId);
            await this.CreateResultAsync(secondStudentId, 15, 15, eventId);

            var firstModel = new ResultViewModel()
            {
                StudentName = "First Name Last Name",
                StudentEmail = "email@email.com",
                Score = "2/10",
            };

            var secondModel = new ResultViewModel()
            {
                StudentName = "First Name Last Name",
                StudentEmail = "email@email.com",
                Score = "15/15",
            };

            var resultModelCollection = await this.service.GetAllResultsByEventIdAsync<ResultViewModel>(eventId, groupName);
            Assert.Equal(firstModel.StudentName, resultModelCollection.Last().StudentName);
            Assert.Equal(firstModel.StudentEmail, resultModelCollection.First().StudentEmail);
            Assert.Equal(firstModel.Score, resultModelCollection.First().Score);
            Assert.Equal(secondModel.StudentName, resultModelCollection.Last().StudentName);
            Assert.Equal(secondModel.StudentEmail, resultModelCollection.Last().StudentEmail);
            Assert.Equal(secondModel.Score, resultModelCollection.Last().Score);
            Assert.Equal(2, resultModelCollection.Count());
            Assert.IsAssignableFrom<IEnumerable<ResultViewModel>>(resultModelCollection);
        }

        [Fact]
        public async Task CreateResultAsyncShouldCreateNewResultInDb()
        {
            var studentId = Guid.NewGuid().ToString();
            var eventInfo = await this.CreateEventAsync("event", DateTime.UtcNow);
            var quizId = eventInfo[1];

            var newResultId = await this.service.CreateResultAsync(studentId, 2, 10, quizId);
            var resultsCount = this.dbContext.Results.ToArray().Count();
            Assert.Equal(1, resultsCount);
            Assert.NotNull(await this.dbContext.Results.FirstOrDefaultAsync(x => x.Id == newResultId));
        }

        private async Task<string> AssignStudentsToGroupAsync(string[] studentIds)
        {
            var creatorId = Guid.NewGuid().ToString();
            var group = new Group() { Name = "Group", CreatorId = creatorId };
            await this.dbContext.Groups.AddAsync(group);
            foreach (var id in studentIds)
            {
                var studentGroup = new StudentGroup() { StudentId = id, GroupId = group.Id };
                await this.dbContext.StudentsGroups.AddAsync(studentGroup);
                var student = await this.dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
                student.StudentsInGroups.Add(studentGroup);
                this.dbContext.Update(student);
            }

            await this.dbContext.SaveChangesAsync();
            return group.Name;
        }

        private async Task CreateResultAsync(string studentId, int points, int maxPoints, string eventId)
        {
            var result = new Result()
            {
                Points = points,
                StudentId = studentId,
                MaxPoints = maxPoints,
                EventId = eventId,
            };

            await this.dbContext.Results.AddAsync(result);

            var @event = await this.dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            @event.Results.Add(result);
            this.dbContext.Update(@event);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<Result>(result).State = EntityState.Detached;
        }

        private async Task<string[]> CreateEventAsync(string name, DateTime activation)
        {
            var creatorId = Guid.NewGuid().ToString();
            var quiz = new Quiz()
            {
                Name = "quiz",
            };

            await this.dbContext.Quizzes.AddAsync(quiz);

            var @event = new Event
            {
                Name = name,
                Status = Status.Pending,
                ActivationDateAndTime = activation,
                DurationOfActivity = TimeSpan.FromMinutes(30),
                CreatorId = creatorId,
                QuizId = quiz.Id,
            };

            await this.dbContext.Events.AddAsync(@event);
            await this.dbContext.SaveChangesAsync();
            return new string[] { @event.Id, quiz.Id };
        }

        private async Task<string> CreateStudentAsync()
        {
            var student = new ApplicationUser()
            {
                FirstName = "First Name",
                LastName = "Last Name",
                Email = "email@email.com",
                UserName = "email@email.com",
            };

            await this.dbContext.Users.AddAsync(student);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<ApplicationUser>(student).State = EntityState.Detached;
            return student.Id;
        }
    }
}
