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
    using QuizHut.Services.Quizzes;
    using QuizHut.Web.ViewModels.Common;
    using QuizHut.Web.ViewModels.Quizzes;
    using Xunit;

    public class QuizzesServiceTests : BaseServiceTests
    {
        private IQuizzesService Service => this.ServiceProvider.GetRequiredService<IQuizzesService>();

        [Fact]
        public async Task CreateQuizAsyncShouldCreateCorrectly()
        {
            var creatorId = Guid.NewGuid().ToString();
            var quizId = await this.Service.CreateQuizAsync("Quiz", "description", 30, creatorId, "123456789");

            var quiz = await this.GetQuizAsync(quizId);

            Assert.NotNull(quiz);
            Assert.Equal("Quiz", quiz.Name);
            Assert.Equal("description", quiz.Description);
            Assert.Equal(30, quiz.Timer);
            Assert.Equal(creatorId, quiz.CreatorId);
            Assert.Equal("123456789", quiz.Password.Content);
        }

        [Fact]
        public async Task CreateQuizAsyncShouldCreateNewPassword()
        {
            var quizId = await this.Service.CreateQuizAsync("Quiz", "description", 30, Guid.NewGuid().ToString(), "123456789");
            var password = await this.DbContext.Passwords.FirstOrDefaultAsync(x => x.QuizId == quizId);

            Assert.NotNull(password);
            Assert.Equal("123456789", password.Content);
        }

        [Fact]
        public async Task DeleteByIdAsyncShouldDeleteQuizCorrectly()
        {
            var quizId = await this.CreateQuizAsync("Quiz", null, "testpassword");

            await this.Service.DeleteByIdAsync(quizId);

            var quiz = await this.DbContext.Quizzes.FirstOrDefaultAsync();

            Assert.Null(quiz);
        }

        [Fact]
        public async Task DeleteByIdAsyncShouldDeletePasswordCorrectly()
        {
            var quizId = await this.CreateQuizAsync("Quiz", null, "testpassword");

            var passwordBeforeQuizIsDeleted = await this.DbContext.Passwords.FirstOrDefaultAsync(x => x.QuizId == quizId);
            this.DbContext.Entry<Password>(passwordBeforeQuizIsDeleted).State = EntityState.Detached;

            await this.Service.DeleteByIdAsync(quizId);

            var passwordAfterQuizIsDeleted = await this.DbContext.Passwords.FirstOrDefaultAsync(x => x.QuizId == quizId);

            Assert.NotNull(passwordBeforeQuizIsDeleted);
            Assert.Null(passwordAfterQuizIsDeleted);
        }

        [Fact]
        public async Task GetQuizIdByPasswordAsyncShouldReturnCorrectQuizId()
        {
            var quizId = await this.CreateQuizAsync("Quiz", null, "testpassword");

            var resultQuizId = await this.Service.GetQuizIdByPasswordAsync("testpassword");

            Assert.NotNull(resultQuizId);
            Assert.Equal(quizId, resultQuizId);
        }

        [Fact]
        public async Task GetQuizNameByIdAsyncShouldReturnCorrectName()
        {
            var quizId = await this.CreateQuizAsync("Quiz", null, "testpassword");

            var quizName = await this.Service.GetQuizNameByIdAsync(quizId);

            Assert.NotNull(quizName);
            Assert.Equal("Quiz", quizName);
        }

        [Fact]
        public async Task GetAllQuizzesCountShouldReturnCorrectCount()
        {
            var firstCreatorId = Guid.NewGuid().ToString();
            var secondCreatorId = Guid.NewGuid().ToString();

            await this.CreateQuizAsync("First Quiz", firstCreatorId);
            await this.CreateQuizAsync("Second Quiz", secondCreatorId);

            var caseWhenCreatorIdIsPassedCount = await this.Service.GetAllQuizzesCountAsync(firstCreatorId);
            var caseWhenNoCreatorIdIsPassedCount = await this.Service.GetAllQuizzesCountAsync();

            Assert.Equal(1, caseWhenCreatorIdIsPassedCount);
            Assert.Equal(2, caseWhenNoCreatorIdIsPassedCount);
        }

        [Fact]
        public async Task GetCreatorIdByQuizIdAsyncShouldReturnCorrectCreatorId()
        {
            var creatorId = Guid.NewGuid().ToString();
            var quizId = await this.CreateQuizAsync("Quiz", creatorId, "testpassword");

            var creatorIdResult = await this.Service.GetCreatorIdByQuizIdAsync(quizId);

            Assert.NotNull(creatorIdResult);
            Assert.Equal(creatorId, creatorIdResult);
        }

        [Fact]
        public async Task AssignQuizToEventAsyncShouldAssignCorrectly()
        {
            var eventId = Guid.NewGuid().ToString();
            var quizId = await this.CreateQuizAsync("Test quiz");

            await this.Service.AssignQuizToEventAsync(eventId, quizId);

            var quiz = await this.GetQuizAsync(quizId);

            Assert.NotNull(quiz.EventId);
            Assert.Equal(eventId, quiz.EventId);
        }

        [Fact]
        public async Task DeleteEventFromQuizAsyncShouldRemoveEventIdCorrectly()
        {
            var eventId = Guid.NewGuid().ToString();
            var quizId = await this.CreateQuizAsync("Test quiz");
            await this.AssignEventToQuizAsync(quizId, eventId);

            var quizAfterAssigningTheEvent = await this.GetQuizAsync(quizId);

            await this.Service.DeleteEventFromQuizAsync(eventId, quizId);

            var quizAfterDeletingTheEvent = await this.GetQuizAsync(quizId);

            Assert.NotNull(quizAfterAssigningTheEvent.EventId);
            Assert.Equal(eventId, quizAfterAssigningTheEvent.EventId);
            Assert.Null(quizAfterDeletingTheEvent.EventId);
        }

        [Fact]
        public async Task UpdateAsyncShouldUpdateCorrectly()
        {
            var quizId = await this.CreateQuizAsync("Test quiz");

            await this.Service.UpdateAsync(quizId, "First Quiz", "Description", 32, "6543211");

            var quizAfterUpdate = await this.GetQuizAsync(quizId);

            Assert.Equal("First Quiz", quizAfterUpdate.Name);
            Assert.Equal("Description", quizAfterUpdate.Description);
            Assert.Equal(32, quizAfterUpdate.Timer);
            Assert.Equal("6543211", quizAfterUpdate.Password.Content);
        }

        [Fact]
        public async Task GetQuizByIdAsyncShouldReturnCorrectModel()
        {
            var quizId = await this.CreateQuizAsync("Test quiz", null, "testquizpass");

            var model = new QuizDetailsViewModel()
            {
                Id = quizId,
                Name = "Test quiz",
                Description = null,
                Timer = null,
                Password = "testquizpass",
                EventName = null,
            };

            var resultModel = await this.Service.GetQuizByIdAsync<QuizDetailsViewModel>(quizId);

            Assert.IsAssignableFrom<QuizDetailsViewModel>(resultModel);
            Assert.Equal(model.Id, resultModel.Id);
            Assert.Equal(model.Name, resultModel.Name);
            Assert.Equal(model.Description, resultModel.Description);
            Assert.Equal(model.Timer, resultModel.Timer);
            Assert.Equal(model.Password, resultModel.Password);
            Assert.Equal(model.EventName, resultModel.EventName);
        }

        [Fact]
        public async Task GetAllByCategoryIdAsyncShouldReturnCorrectModelCollection()
        {
            var creatorId = Guid.NewGuid().ToString();
            var categoryId = Guid.NewGuid().ToString();
            var quizId = await this.CreateQuizAsync("First quiz", creatorId, "testquizpass");
            await this.AddQuizToCategoryAsync(categoryId, quizId);
            await this.CreateQuizAsync("Second quiz");

            var model = new QuizAssignViewModel()
            {
                Id = quizId,
                Name = "First quiz",
                CreatorId = creatorId,
                IsAssigned = false,
            };

            var resultModelCollection = await this.Service.GetAllByCategoryIdAsync<QuizAssignViewModel>(categoryId);

            Assert.IsAssignableFrom<IList<QuizAssignViewModel>>(resultModelCollection);
            Assert.Equal(model.Id, resultModelCollection.First().Id);
            Assert.Equal(model.Name, resultModelCollection.First().Name);
            Assert.Equal(model.CreatorId, resultModelCollection.First().CreatorId);
            Assert.False(resultModelCollection.First().IsAssigned);
        }

        [Fact]
        public async Task GetUnAssignedToCategoryByCreatorIdAsyncShouldReturnCorrectModelCollection()
        {
            var creatorId = Guid.NewGuid().ToString();
            var firstQuizId = await this.CreateQuizAsync("First quiz", creatorId, "testquizpass");

            var secondQuizId = await this.CreateQuizAsync("Second quiz", creatorId);
            var categoryId = Guid.NewGuid().ToString();
            await this.AddQuizToCategoryAsync(categoryId, secondQuizId);

            var model = new QuizAssignViewModel()
            {
                Id = firstQuizId,
                Name = "First quiz",
                CreatorId = creatorId,
                IsAssigned = false,
            };

            var resultModelCollection = await this.Service.GetUnAssignedToCategoryByCreatorIdAsync<QuizAssignViewModel>(categoryId, creatorId);

            Assert.IsAssignableFrom<IList<QuizAssignViewModel>>(resultModelCollection);
            Assert.Equal(model.Id, resultModelCollection.First().Id);
            Assert.Equal(model.Name, resultModelCollection.First().Name);
            Assert.Equal(model.CreatorId, resultModelCollection.First().CreatorId);
            Assert.False(resultModelCollection.First().IsAssigned);
        }

        [Fact]
        public async Task HasUserPermitionShouldReturnTrueIfTheUserIsTheCreator()
        {
            var creatorId = Guid.NewGuid().ToString();
            var quizId = await this.CreateQuizAsync("Test Quiz", creatorId);
            var result = await this.Service.HasUserPermition(creatorId, quizId, true);

            Assert.True(result[0]);
            Assert.True(result[1]);
        }

        [Fact]
        public async Task HasUserPermitionShouldReturnFalseIfUserIdExistsInMemoryCache()
        {
            var quizId = await this.CreateQuizAsync("Test Quiz");
            var isQuizTaken = true;
            var result = await this.Service.HasUserPermition(Guid.NewGuid().ToString(), quizId, isQuizTaken);

            Assert.False(result[0]);
            Assert.False(result[1]);
        }

        [Fact]
        public async Task HasUserPermitionShouldReturnFalseIfTheQuizIsAssignedToUnActiveEvent()
        {
            var creatorId = Guid.NewGuid().ToString();
            var quizId = await this.CreateQuizAsync("Test Quiz", creatorId);
            var eventId = await this.CreateEventAsync(Status.Pending);
            await this.AssignEventToQuizAsync(quizId, eventId);

            var result = await this.Service.HasUserPermition(Guid.NewGuid().ToString(), quizId, false);

            Assert.False(result[0]);
            Assert.False(result[1]);
        }

        [Fact]
        public async Task HasUserPermitionShouldReturnFalseIfTheQuizIsAlreadyTakenFromTheStudent()
        {
            var studentId = Guid.NewGuid().ToString();
            var quizId = await this.CreateQuizAsync("Test Quiz");
            var eventId = await this.CreateEventAsync(Status.Active);
            await this.AssignEventToQuizAsync(quizId, eventId);
            await this.CreateResultAsync(studentId, eventId);

            var result = await this.Service.HasUserPermition(studentId, quizId, false);

            Assert.False(result[0]);
            Assert.False(result[1]);
        }

        [Fact]
        public async Task HasUserPermitionShouldReturnFalseIfStudentIsNotInTheAssignedToTheEventGroup()
        {
            var studentId = Guid.NewGuid().ToString();
            var quizId = await this.CreateQuizAsync("Test Quiz");
            var eventId = await this.CreateEventAsync(Status.Active);
            await this.AssignEventToQuizAsync(quizId, eventId);

            var result = await this.Service.HasUserPermition(studentId, quizId, false);

            Assert.False(result[0]);
            Assert.False(result[1]);
        }

        [Fact]
        public async Task HasUserPermitionShouldReturnTrueIfStudentIsInTheAssignedToTheEventGroup()
        {
            var studentId = Guid.NewGuid().ToString();
            var quizId = await this.CreateQuizAsync("Test Quiz");
            var eventId = await this.CreateEventAsync(Status.Active);
            await this.AssignEventToQuizAsync(quizId, eventId);
            await this.CreateGroupAndAssignStudentAndEventAsync(studentId, eventId);

            var result = await this.Service.HasUserPermition(studentId, quizId, false);

            Assert.True(result[0]);
            Assert.False(result[1]);
        }

        [Fact]
        public async Task GetAllPerPageAsyncShouldReturnCorrectModelCollection()
        {
            var firstQuizId = await this.CreateQuizAsync("First quiz");
            await this.AssignEventToQuizAsync(firstQuizId, Guid.NewGuid().ToString());

            var secondQuizId = await this.CreateQuizAsync("Second quiz");

            var firstModel = new QuizListViewModel()
            {
                Id = firstQuizId,
                Name = "First quiz",
                QuestionsCount = 0,
                Color = ModelCostants.ColorActive,
            };

            var secondModel = new QuizListViewModel()
            {
                Id = secondQuizId,
                Name = "Second quiz",
                QuestionsCount = 0,
                Color = ModelCostants.ColorEnded,
            };

            var resultModelCollection = await this.Service.GetAllPerPageAsync<QuizListViewModel>(1, 2);

            Assert.Equal(2, resultModelCollection.Count());
            Assert.IsAssignableFrom<IList<QuizListViewModel>>(resultModelCollection);

            Assert.Equal(firstModel.Id, resultModelCollection.Last().Id);
            Assert.Equal(firstModel.Name, resultModelCollection.Last().Name);
            Assert.Equal(firstModel.QuestionsCount, resultModelCollection.Last().QuestionsCount);
            Assert.Equal(firstModel.Color, resultModelCollection.Last().Color);

            Assert.Equal(secondModel.Id, resultModelCollection.First().Id);
            Assert.Equal(secondModel.Name, resultModelCollection.First().Name);
            Assert.Equal(secondModel.QuestionsCount, resultModelCollection.First().QuestionsCount);
            Assert.Equal(secondModel.Color, resultModelCollection.First().Color);
        }

        [Fact]
        public async Task GetAllPerPageAsyncShouldReturnCorrectModelCollectionIfCreatorIdIsPassed()
        {
            var creatorId = Guid.NewGuid().ToString();
            await this.CreateQuizAsync("First quiz");
            var secondQuizId = await this.CreateQuizAsync("Second quiz", creatorId);

            var secondModel = new QuizListViewModel()
            {
                Id = secondQuizId,
                Name = "Second quiz",
                QuestionsCount = 0,
                Color = ModelCostants.ColorEnded,
            };

            var resultModelCollection = await this.Service.GetAllPerPageAsync<QuizListViewModel>(1, 2, creatorId);

            Assert.Single(resultModelCollection);
            Assert.IsAssignableFrom<IList<QuizListViewModel>>(resultModelCollection);

            Assert.Equal(secondModel.Id, resultModelCollection.First().Id);
            Assert.Equal(secondModel.Name, resultModelCollection.First().Name);
            Assert.Equal(secondModel.QuestionsCount, resultModelCollection.First().QuestionsCount);
            Assert.Equal(secondModel.Color, resultModelCollection.First().Color);
        }

        [Theory]
        [InlineData(1, 5)]
        [InlineData(1, 1000)]
        public async Task GetAllPerPageAsyncShouldTakeCorrectCountPerPage(int page, int countPerPage)
        {
            for (int i = 0; i < countPerPage * 2; i++)
            {
                await this.CreateQuizAsync($"quiz {i}");
            }

            var resultModelCollection = await this.Service.GetAllPerPageAsync<QuizListViewModel>(page, countPerPage);

            Assert.Equal(countPerPage, resultModelCollection.Count());
        }

        [Fact]
        public async Task GetPerPageByStudentIdFilteredByStatusAsyncShouldSkipCorrectly()
        {
            var firstQuizId = await this.CreateQuizAsync("First quiz");
            await this.AssignEventToQuizAsync(firstQuizId, Guid.NewGuid().ToString());

            await this.CreateQuizAsync("Second quiz");

            var firstModel = new QuizListViewModel()
            {
                Id = firstQuizId,
                Name = "First quiz",
                QuestionsCount = 0,
                Color = ModelCostants.ColorActive,
            };

            var resultModelCollection = await this.Service.GetAllPerPageAsync<QuizListViewModel>(2, 1);

            Assert.Single(resultModelCollection);
            Assert.IsAssignableFrom<IList<QuizListViewModel>>(resultModelCollection);

            Assert.Equal(firstModel.Id, resultModelCollection.First().Id);
            Assert.Equal(firstModel.Name, resultModelCollection.First().Name);
            Assert.Equal(firstModel.QuestionsCount, resultModelCollection.First().QuestionsCount);
            Assert.Equal(firstModel.Color, resultModelCollection.First().Color);
        }

        [Fact]
        public async Task GetAllUnAssignedToEventAsyncShouldReturnCorrectModelCollection()
        {
            var creatorId = Guid.NewGuid().ToString();
            var firstQuizId = await this.CreateQuizAsync("First quiz", creatorId);

            var secondQuizId = await this.CreateQuizAsync("Second quiz", creatorId);

            var firstModel = new QuizAssignViewModel()
            {
                Id = firstQuizId,
                Name = "First quiz",
                CreatorId = creatorId,
                IsAssigned = false,
            };

            var secondModel = new QuizAssignViewModel()
            {
                Id = secondQuizId,
                Name = "Second quiz",
                CreatorId = creatorId,
                IsAssigned = false,
            };

            var resultModelCollection = await this.Service.GetAllUnAssignedToEventAsync<QuizAssignViewModel>();

            Assert.Equal(2, resultModelCollection.Count());
            Assert.IsAssignableFrom<IList<QuizAssignViewModel>>(resultModelCollection);

            Assert.Equal(firstModel.Id, resultModelCollection.Last().Id);
            Assert.Equal(firstModel.Name, resultModelCollection.Last().Name);
            Assert.Equal(firstModel.CreatorId, resultModelCollection.Last().CreatorId);
            Assert.False(resultModelCollection.Last().IsAssigned);

            Assert.Equal(secondModel.Id, resultModelCollection.First().Id);
            Assert.Equal(secondModel.Name, resultModelCollection.First().Name);
            Assert.Equal(secondModel.CreatorId, resultModelCollection.First().CreatorId);
            Assert.False(resultModelCollection.First().IsAssigned);
        }

        [Fact]
        public async Task GetAllUnAssignedToEventAsyncShouldReturnCorrectModelCollectionIfCreatorIdIsPassed()
        {
            var creatorId = Guid.NewGuid().ToString();
            var firstQuizId = await this.CreateQuizAsync("First quiz", creatorId);

            var secondQuizId = await this.CreateQuizAsync("Second quiz");

            var firstModel = new QuizAssignViewModel()
            {
                Id = firstQuizId,
                Name = "First quiz",
                CreatorId = creatorId,
                IsAssigned = false,
            };

            var resultModelCollection = await this.Service.GetAllUnAssignedToEventAsync<QuizAssignViewModel>(creatorId);

            Assert.Single(resultModelCollection);
            Assert.IsAssignableFrom<IList<QuizAssignViewModel>>(resultModelCollection);

            Assert.Equal(firstModel.Id, resultModelCollection.First().Id);
            Assert.Equal(firstModel.Name, resultModelCollection.First().Name);
            Assert.Equal(firstModel.CreatorId, resultModelCollection.First().CreatorId);
            Assert.False(resultModelCollection.First().IsAssigned);
        }

        private async Task<string> CreateQuizAsync(string name, string creatorId = null, string password = null)
        {
            var quiz = new Quiz
            {
                Name = name,
                Description = null,
                Timer = null,
                CreatorId = creatorId ?? Guid.NewGuid().ToString(),
            };

            await this.DbContext.Quizzes.AddAsync(quiz);
            await this.DbContext.SaveChangesAsync();

            var passwordModel = new Password()
            {
                Content = password ?? "123456",
                QuizId = quiz.Id,
            };

            await this.DbContext.Passwords.AddAsync(passwordModel);

            quiz.PasswordId = passwordModel.Id;
            this.DbContext.Quizzes.Update(quiz);
            await this.DbContext.SaveChangesAsync();

            this.DbContext.Entry<Quiz>(quiz).State = EntityState.Detached;
            this.DbContext.Entry<Password>(passwordModel).State = EntityState.Detached;

            return quiz.Id;
        }

        private async Task AssignEventToQuizAsync(string quizId, string eventId)
        {
            var quiz = await this.DbContext.Quizzes.FirstOrDefaultAsync(x => x.Id == quizId);
            quiz.EventId = eventId;
            this.DbContext.Update(quiz);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Quiz>(quiz).State = EntityState.Detached;
        }

        private async Task<Quiz> GetQuizAsync(string id)
        {
            var quiz = await this.DbContext.Quizzes.FirstOrDefaultAsync(x => x.Id == id);
            this.DbContext.Entry<Quiz>(quiz).State = EntityState.Detached;
            return quiz;
        }

        private async Task AddQuizToCategoryAsync(string categoryId, string quizId)
        {
            var quiz = await this.DbContext.Quizzes.FirstOrDefaultAsync(x => x.Id == quizId);
            quiz.CategoryId = categoryId;
            this.DbContext.Update(quiz);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Quiz>(quiz).State = EntityState.Detached;
        }

        private async Task<string> CreateEventAsync(Status status)
        {
            var @event = new Event
            {
                Name = "Event",
                Status = status,
                ActivationDateAndTime = DateTime.Now,
                DurationOfActivity = TimeSpan.FromMinutes(30),
                CreatorId = Guid.NewGuid().ToString(),
            };

            await this.DbContext.Events.AddAsync(@event);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Event>(@event).State = EntityState.Detached;
            return @event.Id;
        }

        private async Task CreateResultAsync(string studentId, string eventId)
        {
            var result = new Result()
            {
                Points = 10,
                StudentId = studentId,
                MaxPoints = 10,
                EventId = eventId,
            };

            await this.DbContext.Results.AddAsync(result);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Result>(result).State = EntityState.Detached;
        }

        private async Task CreateGroupAndAssignStudentAndEventAsync(string studentId, string eventId)
        {
            var group = new Group()
            {
                Name = "group",
                CreatorId = Guid.NewGuid().ToString(),
            };

            await this.DbContext.Groups.AddAsync(group);

            var studentGroup = new StudentGroup() { StudentId = studentId, GroupId = group.Id };
            await this.DbContext.StudentsGroups.AddAsync(studentGroup);

            var eventGroup = new EventGroup() { EventId = eventId, GroupId = group.Id };
            await this.DbContext.EventsGroups.AddAsync(eventGroup);

            await this.DbContext.SaveChangesAsync();

            this.DbContext.Entry<EventGroup>(eventGroup).State = EntityState.Detached;
            this.DbContext.Entry<Group>(group).State = EntityState.Detached;
            this.DbContext.Entry<StudentGroup>(studentGroup).State = EntityState.Detached;
        }
    }
}
