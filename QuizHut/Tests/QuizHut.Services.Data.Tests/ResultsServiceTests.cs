namespace QuizHut.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Models;
    using QuizHut.Services.Results;
    using QuizHut.Web.ViewModels.Results;
    using Xunit;

    public class ResultsServiceTests : BaseServiceTests
    {
        private IResultsService Service => this.ServiceProvider.GetRequiredService<IResultsService>();

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

            var resultModelCollection = await this.Service.GetAllByStudentIdAsync<ScoreViewModel>(studentId);

            Assert.Equal(firstModel.EventId, resultModelCollection.Last().EventId);
            Assert.Equal(firstModel.Score, resultModelCollection.Last().Score);
            Assert.Equal(secondModel.EventId, resultModelCollection.First().EventId);
            Assert.Equal(secondModel.Score, resultModelCollection.First().Score);
            Assert.Equal(2, resultModelCollection.Count());
            Assert.IsAssignableFrom<IEnumerable<ScoreViewModel>>(resultModelCollection);
        }

        [Fact]
        public async Task GetAllResultsByEventAndGroupPerPageAsyncShouldReturnCorrectModelCollection()
        {
            var firstStudentId = await this.CreateStudentAsync();
            var secondStudentId = await this.CreateStudentAsync();

            var activationDate = DateTime.UtcNow;
            var eventInfo = await this.CreateEventAsync("First event", activationDate);
            var eventId = eventInfo[0];
            var groupId = await this.AssignStudentsToGroupAsync(new string[] { firstStudentId, secondStudentId });

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

            var resultModelCollection = await this.Service.GetAllResultsByEventAndGroupPerPageAsync<ResultViewModel>(eventId, groupId, 1, 2);

            Assert.Equal(firstModel.StudentName, resultModelCollection.Last().StudentName);
            Assert.Equal(firstModel.StudentEmail, resultModelCollection.First().StudentEmail);
            Assert.Equal(firstModel.Score, resultModelCollection.First().Score);
            Assert.Equal(secondModel.StudentName, resultModelCollection.Last().StudentName);
            Assert.Equal(secondModel.StudentEmail, resultModelCollection.Last().StudentEmail);
            Assert.Equal(secondModel.Score, resultModelCollection.Last().Score);
            Assert.Equal(2, resultModelCollection.Count());
        }

        [Fact]
        public async Task GetAllResultsByEventAndGroupPerPageAsyncShouldSkipCorrectly()
        {
            var firstStudentId = await this.CreateStudentAsync();
            var secondStudentId = await this.CreateStudentAsync();

            var activationDate = DateTime.UtcNow;
            var eventInfo = await this.CreateEventAsync("First event", activationDate);
            var eventId = eventInfo[0];
            var groupId = await this.AssignStudentsToGroupAsync(new string[] { firstStudentId, secondStudentId });

            await this.CreateResultAsync(firstStudentId, 2, 10, eventId);
            await this.CreateResultAsync(secondStudentId, 15, 15, eventId);

            var secondModel = new ResultViewModel()
            {
                StudentName = "First Name Last Name",
                StudentEmail = "email@email.com",
                Score = "15/15",
            };

            var resultModelCollection = await this.Service.GetAllResultsByEventAndGroupPerPageAsync<ResultViewModel>(eventId, groupId, 2, 1);

            Assert.Equal(secondModel.StudentName, resultModelCollection.Last().StudentName);
            Assert.Equal(secondModel.StudentEmail, resultModelCollection.Last().StudentEmail);
            Assert.Equal(secondModel.Score, resultModelCollection.Last().Score);
            Assert.Single(resultModelCollection);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 10)]
        public async Task GetAllResultsByEventAndGroupPerPageAsyncShouldTakeCorrectCountPerPage(int page, int countPerPage)
        {
            var groupId = await this.CreateGroupAsync();
            var eventInfo = await this.CreateEventAsync("First Event", DateTime.UtcNow);

            for (int i = 0; i < countPerPage * 2; i++)
            {
                var studentId = await this.CreateStudentAsync();
                await this.AssignStudentToGroupAsync(studentId, groupId);
                await this.CreateResultAsync(studentId, 5, 10, eventInfo[0]);
            }

            var resultModelCollection = await this.Service.GetAllResultsByEventAndGroupPerPageAsync<ResultViewModel>(eventInfo[0], groupId, page, countPerPage);

            Assert.Equal(countPerPage, resultModelCollection.Count());
        }

        [Fact]
        public async Task GetAllResultsByEventAndGroupCountShouldReturnCorrectCount()
        {
            var firstStudentId = await this.CreateStudentAsync();
            var secondStudentId = await this.CreateStudentAsync();
            var eventInfo = await this.CreateEventAsync("First Event", DateTime.UtcNow);
            var groupId = await this.CreateGroupAsync();
            await this.AssignStudentToGroupAsync(firstStudentId, groupId);
            await this.AssignStudentToGroupAsync(secondStudentId, groupId);

            await this.CreateResultAsync(firstStudentId, 2, 10, eventInfo[0]);
            await this.CreateResultAsync(firstStudentId, 5, 10, eventInfo[0]);

            var count = await this.Service.GetAllResultsByEventAndGroupCountAsync(eventInfo[0], groupId);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task CreateResultAsyncShouldCreateNewResultInDb()
        {
            var studentId = Guid.NewGuid().ToString();
            var eventInfo = await this.CreateEventAsync("event", DateTime.UtcNow);
            var quizId = eventInfo[1];

            var newResultId = await this.Service.CreateResultAsync(studentId, 2, 10, quizId);
            var resultsCount = this.DbContext.Results.ToArray().Count();
            Assert.Equal(1, resultsCount);
            Assert.NotNull(await this.DbContext.Results.FirstOrDefaultAsync(x => x.Id == newResultId));
        }

        [Fact]
        public async Task GetResultsCountByStudentIdShouldReturnCorrectCountWithEmptyCriteriaAndText()
        {
            var studentId = await this.CreateStudentAsync();
            var firstEventInfo = await this.CreateEventAsync("First Event", DateTime.UtcNow);
            var secondEventInfo = await this.CreateEventAsync("Second Event", DateTime.UtcNow);

            await this.CreateResultAsync(studentId, 2, 10, firstEventInfo[0]);
            await this.CreateResultAsync(studentId, 5, 10, secondEventInfo[0]);

            var count = await this.Service.GetResultsCountByStudentIdAsync(studentId);

            Assert.Equal(2, count);
        }

        [Theory]
        [InlineData("EventName", null)]
        [InlineData("QuizName", null)]
        [InlineData("EventName", "event")]
        [InlineData("QuizName", "quiz")]
        [InlineData("EventName", "EvEnt")]
        [InlineData("QuizName", "QuIz")]
        [InlineData("EventName", "n")]
        [InlineData("QuizName", "Z")]
        public async Task GetResultsCountByStudentIdShouldReturnCorrectCountWithSearchCriteriaAndSerchTextPassed(string searchCriteria, string searchText)
        {
            var studentId = await this.CreateStudentAsync();
            var firstEventInfo = await this.CreateEventAsync("First Event", DateTime.UtcNow);
            var secondEventInfo = await this.CreateEventAsync("Second Event", DateTime.UtcNow);

            await this.CreateResultAsync(studentId, 2, 10, firstEventInfo[0]);
            await this.CreateResultAsync(studentId, 5, 10, secondEventInfo[0]);

            var count = await this.Service.GetResultsCountByStudentIdAsync(studentId, searchCriteria, searchText);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetPerPageByStudentIdAsyncShouldReturnCorrectModelCollection()
        {
            var studentId = await this.CreateStudentAsync();
            var firstEventDate = DateTime.UtcNow;
            var secondEventDate = DateTime.UtcNow;

            var firstEventInfo = await this.CreateEventAsync("First Event", firstEventDate);
            var secondEventInfo = await this.CreateEventAsync("Second Event", secondEventDate);

            await this.CreateResultAsync(studentId, 2, 10, firstEventInfo[0]);
            await this.CreateResultAsync(studentId, 5, 10, secondEventInfo[0]);

            var firstModel = new StudentResultViewModel()
            {
                Event = "First Event",
                Quiz = "quiz",
                EventActivationDateAndTime = firstEventDate,
                Score = "2/10",
            };

            var secondModel = new StudentResultViewModel()
            {
                Event = "Second Event",
                Quiz = "quiz",
                EventActivationDateAndTime = secondEventDate,
                Score = "5/10",
            };

            var resultModelCollection = await this.Service.GetPerPageByStudentIdAsync<StudentResultViewModel>(studentId, 1, 2);

            Assert.Equal(firstModel.Event, resultModelCollection.Last().Event);
            Assert.Equal(firstModel.Quiz, resultModelCollection.Last().Quiz);
            Assert.Equal(firstModel.Date, resultModelCollection.Last().Date);
            Assert.Equal(firstModel.Score, resultModelCollection.Last().Score);
            Assert.Equal(secondModel.Event, resultModelCollection.First().Event);
            Assert.Equal(secondModel.Quiz, resultModelCollection.First().Quiz);
            Assert.Equal(secondModel.Date, resultModelCollection.First().Date);
            Assert.Equal(secondModel.Score, resultModelCollection.First().Score);
            Assert.Equal(2, resultModelCollection.Count());
        }

        [Fact]
        public async Task GetPerPageByStudentIdAsyncShouldSkipCorrectly()
        {
            var studentId = await this.CreateStudentAsync();
            var firstEventDate = DateTime.UtcNow;
            var secondEventDate = DateTime.UtcNow;

            var firstEventInfo = await this.CreateEventAsync("First Event", firstEventDate);
            var secondEventInfo = await this.CreateEventAsync("Second Event", secondEventDate);

            await this.CreateResultAsync(studentId, 2, 10, firstEventInfo[0]);
            await this.CreateResultAsync(studentId, 5, 10, secondEventInfo[0]);

            var firstModel = new StudentResultViewModel()
            {
                Event = "First Event",
                Quiz = "quiz",
                EventActivationDateAndTime = firstEventDate,
                Score = "2/10",
            };

            var resultModelCollection = await this.Service.GetPerPageByStudentIdAsync<StudentResultViewModel>(studentId, 2, 1);

            Assert.Single(resultModelCollection);
            Assert.Equal(firstModel.Event, resultModelCollection.First().Event);
            Assert.Equal(firstModel.Quiz, resultModelCollection.First().Quiz);
            Assert.Equal(firstModel.Date, resultModelCollection.First().Date);
            Assert.Equal(firstModel.Score, resultModelCollection.First().Score);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 10)]
        public async Task GetPerPageByStudentIdAsyncShouldTakeCorrectCountPerPage(int page, int countPerPage)
        {
            var studentId = await this.CreateStudentAsync();

            for (int i = 0; i < countPerPage * 2; i++)
            {
                var eventDate = DateTime.UtcNow;
                var eventInfo = await this.CreateEventAsync("First Event", eventDate);
                await this.CreateResultAsync(studentId, 5, 10, eventInfo[0]);
            }

            var resultModelCollection = await this.Service.GetPerPageByStudentIdAsync<StudentResultViewModel>(studentId, page, countPerPage);

            Assert.Equal(countPerPage, resultModelCollection.Count());
        }

        [Fact]
        public async Task GetQuizNameByEventIdAndStudentIdAsyncShouldReturnCorrectName()
        {
            var studentId = await this.CreateStudentAsync();
            var eventDate = DateTime.UtcNow;
            var eventInfo = await this.CreateEventAsync("First Event", eventDate);
            await this.CreateResultAsync(studentId, 5, 10, eventInfo[0]);

            var quizName = await this.Service.GetQuizNameByEventIdAndStudentIdAsync(eventInfo[0], studentId);
            Assert.Equal("quiz", quizName);
        }

        private async Task<string> AssignStudentsToGroupAsync(string[] studentIds)
        {
            var creatorId = Guid.NewGuid().ToString();
            var group = new Group() { Name = "Group", CreatorId = creatorId };
            await this.DbContext.Groups.AddAsync(group);
            foreach (var id in studentIds)
            {
                var studentGroup = new StudentGroup() { StudentId = id, GroupId = group.Id };
                await this.DbContext.StudentsGroups.AddAsync(studentGroup);
                var student = await this.DbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
                student.StudentsInGroups.Add(studentGroup);
                this.DbContext.Update(student);
            }

            await this.DbContext.SaveChangesAsync();
            return group.Id;
        }

        private async Task AssignStudentToGroupAsync(string studentId, string groupId)
        {
            var creatorId = Guid.NewGuid().ToString();
            var group = await this.DbContext.Groups.Where(x => x.Id == groupId).FirstOrDefaultAsync();

            var studentGroup = new StudentGroup() { StudentId = studentId, GroupId = group.Id };
            await this.DbContext.StudentsGroups.AddAsync(studentGroup);
            var student = await this.DbContext.Users.FirstOrDefaultAsync(x => x.Id == studentId);
            student.StudentsInGroups.Add(studentGroup);
            this.DbContext.Update(student);

            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Group>(group).State = EntityState.Detached;
            this.DbContext.Entry<ApplicationUser>(student).State = EntityState.Detached;
        }

        private async Task CreateResultAsync(string studentId, int points, int maxPoints, string eventId)
        {
            var @event = await this.DbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);

            var result = new Result()
            {
                Points = points,
                StudentId = studentId,
                MaxPoints = maxPoints,
                EventId = eventId,
                EventName = @event.Name,
                EventActivationDateAndTime = @event.ActivationDateAndTime,
                QuizName = @event.QuizName,
            };

            await this.DbContext.Results.AddAsync(result);

            @event.Results.Add(result);
            this.DbContext.Update(@event);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Result>(result).State = EntityState.Detached;
        }

        private async Task<string[]> CreateEventAsync(string name, DateTime activation)
        {
            var creatorId = Guid.NewGuid().ToString();
            var quiz = new Quiz()
            {
                Name = "quiz",
            };

            await this.DbContext.Quizzes.AddAsync(quiz);

            var @event = new Event
            {
                Name = name,
                Status = Status.Pending,
                ActivationDateAndTime = activation,
                DurationOfActivity = TimeSpan.FromMinutes(30),
                CreatorId = creatorId,
                QuizId = quiz.Id,
                QuizName = quiz.Name,
            };

            await this.DbContext.Events.AddAsync(@event);
            await this.DbContext.SaveChangesAsync();
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

            await this.DbContext.Users.AddAsync(student);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<ApplicationUser>(student).State = EntityState.Detached;
            return student.Id;
        }

        private async Task<string> CreateGroupAsync()
        {
            var group = new Group()
            {
                Name = "Group",
            };

            await this.DbContext.Groups.AddAsync(group);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Group>(group).State = EntityState.Detached;
            return group.Id;
        }
    }
}
