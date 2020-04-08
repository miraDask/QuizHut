namespace QuizHut.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using QuizHut.Common.Hubs;
    using QuizHut.Data;
    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Models;
    using QuizHut.Data.Repositories;
    using QuizHut.Services.Events;
    using QuizHut.Services.EventsGroups;
    using QuizHut.Services.Mapping;
    using QuizHut.Services.Messaging;
    using QuizHut.Services.Quizzes;
    using QuizHut.Services.ScheduledJobsService;
    using QuizHut.Web.ViewModels.Events;
    using Xunit;

    public class EventsServiceTests
    {
        private readonly ApplicationDbContext dbContext;
        private readonly EfDeletableEntityRepository<Event> eventsRepository;
        private readonly QuizzesService quizzesService;
        private readonly Mock<IHubContext<QuizHub>> hubContext;
        private readonly EventsService service;

        public EventsServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            this.dbContext = new ApplicationDbContext(options);
            this.eventsRepository = new EfDeletableEntityRepository<Event>(this.dbContext);
            this.quizzesService = new QuizzesService(
                new EfDeletableEntityRepository<Quiz>(this.dbContext),
                new EfRepository<Password>(this.dbContext));
            this.hubContext = new Mock<IHubContext<QuizHub>>();

            var scheduledJobsService = new ScheduledJobsService(
                new EfDeletableEntityRepository<ScheduledJob>(this.dbContext),
                this.eventsRepository,
                this.hubContext.Object);

            this.service = new EventsService(
                this.eventsRepository,
                this.quizzesService,
                new EventsGroupsService(new EfDeletableEntityRepository<EventGroup>(this.dbContext)),
                scheduledJobsService,
                new SendGridEmailSender("someFalseKey"),
                this.hubContext.Object);

            AutoMapperConfig.RegisterMappings(typeof(EventListViewModel).GetTypeInfo().Assembly);
        }

        [Fact]
        public async Task DeleteAsyncShouldDeleteCorrectly()
        {
            var creatorId = Guid.NewGuid().ToString();
            var firstEventId = await this.CreateEventAsync("First Event", DateTime.UtcNow, creatorId);
            await this.service.DeleteAsync(firstEventId);

            var eventsCount = this.dbContext.Events.Where(x => !x.IsDeleted).ToArray().Count();
            var @event = await this.dbContext.Events.FindAsync(firstEventId);
            Assert.Equal(0, eventsCount);
            Assert.True(@event.IsDeleted);
        }

        [Fact]
        public async Task GetAllByCreatorIdAsyncShouldReturnCorrectModelCollection()
        {
            var creatorId = Guid.NewGuid().ToString();
            var firstEventDate = DateTime.UtcNow;
            var secondEventDate = DateTime.UtcNow;

            var firstEventId = await this.CreateEventAsync("First Event", firstEventDate, creatorId);
            var secondEventId = await this.CreateEventAsync("Second Event", secondEventDate, creatorId);

            var firstModel = new EventListViewModel()
            {
                Id = firstEventId,
                Name = "First Event",
                IsDeleted = false,
                ActivationDateAndTime = firstEventDate,
                DurationOfActivity = TimeSpan.FromMinutes(30),
                Status = Status.Pending.ToString(),
            };

            var secondModel = new EventListViewModel()
            {
                Id = secondEventId,
                Name = "Second Event",
                IsDeleted = false,
                ActivationDateAndTime = secondEventDate,
                DurationOfActivity = TimeSpan.FromMinutes(30),
                Status = Status.Pending.ToString(),
            };

            var firstModelExpectedStartDate = $"{firstEventDate.ToLocalTime().Date.ToString("dd/MM/yyyy")}";
            var firstModelExpectedDuration = $"{firstEventDate.ToLocalTime().Hour.ToString("D2")}" +
                $":{firstEventDate.ToLocalTime().Minute.ToString("D2")}" +
               $" - {firstEventDate.ToLocalTime().Add(TimeSpan.FromMinutes(30)).Hour.ToString("D2")}" +
               $":{firstEventDate.ToLocalTime().Add(TimeSpan.FromMinutes(30)).Minute.ToString("D2")}";

            var secondModelExpectedStartDate = $"{secondEventDate.ToLocalTime().Date.ToString("dd/MM/yyyy")}";
            var secondModelExpectedDuration = $"{secondEventDate.ToLocalTime().Hour.ToString("D2")}" +
                $":{secondEventDate.ToLocalTime().Minute.ToString("D2")}" +
               $" - {secondEventDate.ToLocalTime().Add(TimeSpan.FromMinutes(30)).Hour.ToString("D2")}" +
               $":{secondEventDate.ToLocalTime().Add(TimeSpan.FromMinutes(30)).Minute.ToString("D2")}";

            var resultModelCollection = await this.service.GetAllByCreatorIdAsync<EventListViewModel>(creatorId);

            Assert.Equal(firstModel.Id, resultModelCollection.Last().Id);
            Assert.Equal(firstModel.Name, resultModelCollection.Last().Name);
            Assert.Equal(firstModel.IsDeleted, resultModelCollection.Last().IsDeleted);
            Assert.Equal(firstModel.ActivationDateAndTime, resultModelCollection.Last().ActivationDateAndTime);
            Assert.Equal(firstModel.DurationOfActivity, resultModelCollection.Last().DurationOfActivity);
            Assert.Equal(firstModel.Status, resultModelCollection.Last().Status);
            Assert.Equal(firstModelExpectedStartDate, resultModelCollection.Last().StartDate);
            Assert.Equal(firstModelExpectedDuration, resultModelCollection.Last().Duration);

            Assert.Equal(secondModel.Id, resultModelCollection.First().Id);
            Assert.Equal(secondModel.Name, resultModelCollection.First().Name);
            Assert.Equal(secondModel.IsDeleted, resultModelCollection.First().IsDeleted);
            Assert.Equal(secondModel.ActivationDateAndTime, resultModelCollection.First().ActivationDateAndTime);
            Assert.Equal(secondModel.DurationOfActivity, resultModelCollection.First().DurationOfActivity);
            Assert.Equal(secondModel.Status, resultModelCollection.First().Status);
            Assert.Equal(secondModelExpectedDuration, resultModelCollection.First().Duration);
            Assert.Equal(secondModelExpectedStartDate, resultModelCollection.First().StartDate);

            Assert.Equal(2, resultModelCollection.Count());
        }

        [Fact]
        public async Task GetEventsCountByCreatorIdAndCountShouldReturnCorrectCount()
        {
            var creatorId = Guid.NewGuid().ToString();

            await this.CreateEventAsync("First Event", DateTime.UtcNow, creatorId);
            await this.CreateEventAsync("Second Event", DateTime.UtcNow, creatorId);

            var count = this.service.GetEventsCountByCreatorIdAndStatus(Status.Pending, creatorId);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetEventsCountByStudentIdAndStatusShouldReturnCorrectCount()
        {
            var creatorId = Guid.NewGuid().ToString();
            var studentId = Guid.NewGuid().ToString();

            await this.CreateEventAsync("First Event", DateTime.UtcNow, creatorId);
            var secondEventId = await this.CreateEventAsync("Second Event", DateTime.UtcNow, creatorId);
            await this.AssignStudentToEvent(studentId, secondEventId);
            var thirdEventId = await this.CreateEventAsync("Third Event", DateTime.UtcNow, creatorId);
            await this.AssignStudentToEvent(studentId, thirdEventId);
            var count = this.service.GetEventsCountByStudentIdAndStatus(studentId, Status.Pending);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task AssignGroupsToEventAsyncShouldAssignGroupsCorrectly()
        {
            var creatorId = Guid.NewGuid().ToString();
            var eventId = await this.CreateEventAsync("First Event", DateTime.UtcNow, creatorId);
            var groupsIds = new List<string>();

            for (int i = 0; i < 5; i++)
            {
                var groupId = await this.CreateGroupAsync();
                groupsIds.Add(groupId);
            }

            await this.service.AssignGroupsToEventAsync(groupsIds, eventId);

            var assignedGroupsIds = await this.dbContext.Events
                .Where(x => x.Id == eventId)
                .Select(x => x.EventsGroups.Select(x => x.GroupId))
                .FirstOrDefaultAsync();

            foreach (var id in groupsIds)
            {
                Assert.Contains(id, assignedGroupsIds);
            }
        }

        [Fact]
        public async Task GetAllEventsCountShouldReturnCorrectCount()
        {
            var firstCreatorId = Guid.NewGuid().ToString();
            var secondCreatorId = Guid.NewGuid().ToString();

            await this.CreateEventAsync("First Event", DateTime.UtcNow, firstCreatorId);
            await this.CreateEventAsync("Second Event", DateTime.UtcNow, secondCreatorId);

            var caseWhenCreatorIdIsPassedCount = this.service.GetAllEventsCount(firstCreatorId);
            var caseWhenNoCreatorIdIsPassedCount = this.service.GetAllEventsCount();

            Assert.Equal(1, caseWhenCreatorIdIsPassedCount);
            Assert.Equal(2, caseWhenNoCreatorIdIsPassedCount);
        }

        // [Fact]
        // public async Task UpdateAsyncShouldUpdateEventCorrectly()
        // {
        // var creatorId = Guid.NewGuid().ToString();
        // var eventId = await this.CreateEventAsync("Event", DateTime.UtcNow, creatorId);
        // var newEventName = "Test Event";
        // var newEventActivationDate = DateTime.Now.AddDays(1);
        // var activeFrom = newEventActivationDate.Add(TimeSpan.FromMinutes(2));
        // var activeTo = newEventActivationDate.Add(TimeSpan.FromHours(1));

        // await this.service.UpdateAsync(eventId, newEventName, newEventActivationDate.Date.ToString(), activeFrom.TimeOfDay.ToString(), activeTo.TimeOfDay.ToString());

        // var expectedEventDuration = activeTo - activeFrom;

        // var updatedEvent = await this.dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);

        // Assert.Equal(newEventName, updatedEvent.Name);
        // Assert.Equal(newEventActivationDate, updatedEvent.ActivationDateAndTime);
        // Assert.Equal(expectedEventDuration, updatedEvent.DurationOfActivity);
        // }
        private async Task<string> CreateEventAsync(string name, DateTime activation, string creatorId)
        {
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
                QuizName = quiz.Name,
            };

            await this.dbContext.Events.AddAsync(@event);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<Quiz>(quiz).State = EntityState.Detached;
            this.dbContext.Entry<Event>(@event).State = EntityState.Detached;
            return @event.Id;
        }

        private async Task AssignStudentToEvent(string studentId, string eventId)
        {
            var groupId = await this.CreateGroupAsync();
            var studentGroup = new StudentGroup() { StudentId = studentId, GroupId = groupId };
            await this.dbContext.StudentsGroups.AddAsync(studentGroup);
            await this.CreateEventGroupAsync(eventId, groupId);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<StudentGroup>(studentGroup).State = EntityState.Detached;
        }

        private async Task<string> CreateGroupAsync()
        {
            var group = new Group()
            {
                Name = "New Group",
                CreatorId = Guid.NewGuid().ToString(),
            };

            await this.dbContext.Groups.AddAsync(group);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<Group>(group).State = EntityState.Detached;
            return group.Id;
        }

        private async Task<EventGroup> CreateEventGroupAsync(string eventId, string groupId)
        {
            var eventGroup = new EventGroup() { EventId = eventId, GroupId = groupId };
            await this.dbContext.EventsGroups.AddAsync(eventGroup);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<EventGroup>(eventGroup).State = EntityState.Detached;
            return eventGroup;
        }
    }
}
