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
    using QuizHut.Services.Events;
    using QuizHut.Web.ViewModels.Events;
    using Xunit;

    public class EventsServiceTests : BaseServiceTests
    {
        private IEventsService Service => this.ServiceProvider.GetRequiredService<IEventsService>();

        [Fact]
        public async Task DeleteAsyncShouldDeleteCorrectly()
        {
            var creatorId = Guid.NewGuid().ToString();
            var firstEventId = await this.CreateEventAsync("First Event", DateTime.UtcNow, creatorId);
            await this.Service.DeleteAsync(firstEventId);

            var eventsCount = this.DbContext.Events.Where(x => !x.IsDeleted).ToArray().Count();
            var @event = await this.DbContext.Events.FindAsync(firstEventId);
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

            var resultModelCollection = await this.Service.GetAllByCreatorIdAsync<EventListViewModel>(creatorId);

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
        public async Task GetPerPageByStudentIdFilteredByStatusAsyncShouldReturnCorrectModelCollection()
        {
            var creatorId = Guid.NewGuid().ToString();
            var firstEventDate = DateTime.UtcNow;
            var secondEventDate = DateTime.UtcNow;

            var firstEventId = await this.CreateEventAsync("First Event", firstEventDate, creatorId);
            var secondEventId = await this.CreateEventAsync("Second Event", secondEventDate, creatorId);

            var studentId = Guid.NewGuid().ToString();
            await this.AssignStudentToEvent(studentId, firstEventId);
            await this.AssignStudentToEvent(studentId, secondEventId);

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

            var resultModelCollection = await this.Service.GetPerPageByStudentIdFilteredByStatusAsync<EventListViewModel>(
                Status.Pending, studentId, 1, 2, false);

            Assert.Equal(2, resultModelCollection.Count());
            Assert.IsAssignableFrom<IList<EventListViewModel>>(resultModelCollection);

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
        }

        [Fact]
        public async Task GetPerPageByStudentIdFilteredByStatusAsyncShouldFilterCorrectlyWhenWithDeletedConditionIsTrue()
        {
            var creatorId = Guid.NewGuid().ToString();

            var firstEventId = await this.CreateEventAsync("First Event", DateTime.UtcNow, creatorId);
            var secondEventId = await this.CreateEventAsync("Second Event", DateTime.UtcNow, creatorId);
            var thirdEventId = await this.CreateEventAsync("Third Event", DateTime.UtcNow, creatorId);

            var studentId = Guid.NewGuid().ToString();
            await this.AssignStudentToEvent(studentId, firstEventId);
            await this.AssignStudentToEvent(studentId, secondEventId);
            await this.AssignStudentToEvent(studentId, thirdEventId);

            await this.Service.DeleteAsync(thirdEventId);

            var resultModelCollection = await this.Service.GetPerPageByStudentIdFilteredByStatusAsync<EventListViewModel>(
                Status.Pending, studentId, 1, 3, true);

            Assert.Equal(3, resultModelCollection.Count());
        }

        [Fact]
        public async Task GetPerPageByStudentIdFilteredByStatusAsyncShouldFilterCorrectlyWhenWithDeletedConditionIsFalse()
        {
            var creatorId = Guid.NewGuid().ToString();

            var firstEventId = await this.CreateEventAsync("First Event", DateTime.UtcNow, creatorId);
            var secondEventId = await this.CreateEventAsync("Second Event", DateTime.UtcNow, creatorId);
            var thirdEventId = await this.CreateEventAsync("Third Event", DateTime.UtcNow, creatorId);

            var studentId = Guid.NewGuid().ToString();
            await this.AssignStudentToEvent(studentId, firstEventId);
            await this.AssignStudentToEvent(studentId, secondEventId);
            await this.AssignStudentToEvent(studentId, thirdEventId);

            await this.Service.DeleteAsync(thirdEventId);

            var resultModelCollection = await this.Service.GetPerPageByStudentIdFilteredByStatusAsync<EventListViewModel>(
                Status.Pending, studentId, 1, 3, false);

            Assert.Equal(2, resultModelCollection.Count());
        }

        [Theory]
        [InlineData(Status.Active)]
        [InlineData(Status.Ended)]
        [InlineData(Status.Pending)]
        public async Task GetPerPageByStudentIdFilteredByStatusAsyncShouldFilterCorrectlyByStatus(Status status)
        {
            var creatorId = Guid.NewGuid().ToString();

            var firstEventId = await this.CreateEventAsync("First Event", DateTime.UtcNow, creatorId);
            await this.ChangeEventStatus(firstEventId, status);
            var secondEventId = await this.CreateEventAsync("Second Event", DateTime.UtcNow, creatorId);
            await this.ChangeEventStatus(secondEventId, status);
            var thirdEventId = await this.CreateEventAsync("Third Event", DateTime.UtcNow, creatorId);
            await this.ChangeEventStatus(thirdEventId, status);

            var studentId = Guid.NewGuid().ToString();
            await this.AssignStudentToEvent(studentId, firstEventId);
            await this.AssignStudentToEvent(studentId, secondEventId);
            await this.AssignStudentToEvent(studentId, thirdEventId);

            var resultModelCollection = await this.Service.GetPerPageByStudentIdFilteredByStatusAsync<EventListViewModel>(
                status, studentId, 1, 3, false);

            Assert.Equal(3, resultModelCollection.Count());
        }

        [Fact]
        public async Task GetPerPageByStudentIdFilteredByStatusAsyncShouldFilterCorrectlyWhenStatusActiveIsPassed()
        {
            var creatorId = Guid.NewGuid().ToString();

            var firstEventId = await this.CreateEventAsync("First Event", DateTime.UtcNow, creatorId);
            await this.ChangeEventStatus(firstEventId, Status.Active);
            var secondEventId = await this.CreateEventAsync("Second Event", DateTime.UtcNow, creatorId);
            await this.ChangeEventStatus(secondEventId, Status.Active);
            var thirdEventId = await this.CreateEventAsync("Third Event", DateTime.UtcNow, creatorId);
            await this.ChangeEventStatus(thirdEventId, Status.Active);

            var studentId = Guid.NewGuid().ToString();
            await this.AssignStudentToEvent(studentId, firstEventId);
            await this.AssignStudentToEvent(studentId, secondEventId);
            await this.AssignStudentToEvent(studentId, thirdEventId);

            var result = new Result()
            {
                Points = 2,
                MaxPoints = 3,
                StudentId = studentId,
                EventId = thirdEventId,
            };
            await this.DbContext.Results.AddAsync(result);
            await this.DbContext.SaveChangesAsync();

            var resultModelCollection = await this.Service.GetPerPageByStudentIdFilteredByStatusAsync<EventListViewModel>(
                Status.Active, studentId, 1, 3, false);

            Assert.Equal(2, resultModelCollection.Count());
        }

        [Theory]
        [InlineData(1, 5)]
        [InlineData(1, 1000)]
        public async Task GetPerPageByStudentIdFilteredByStatusAsyncShouldTakeCorrectCountPerPage(int page, int countPerPage)
        {
            var creatorId = Guid.NewGuid().ToString();
            var studentId = Guid.NewGuid().ToString();

            for (int i = 0; i < countPerPage * 2; i++)
            {
                var eventId = await this.CreateEventAsync("Event", DateTime.UtcNow, creatorId);
                await this.AssignStudentToEvent(studentId, eventId);
            }

            var resultModelCollection = await this.Service.GetPerPageByStudentIdFilteredByStatusAsync<EventListViewModel>(
                Status.Pending, studentId, page, countPerPage, false);

            Assert.Equal(countPerPage, resultModelCollection.Count());
        }

        [Fact]
        public async Task GetPerPageByStudentIdFilteredByStatusAsyncShouldSkipCorrectly()
        {
            var creatorId = Guid.NewGuid().ToString();
            var firstEventDate = DateTime.UtcNow;
            var firstEventId = await this.CreateEventAsync("First Event", firstEventDate, creatorId);
            var secondEventId = await this.CreateEventAsync("Second Event", DateTime.UtcNow, creatorId);

            var studentId = Guid.NewGuid().ToString();
            await this.AssignStudentToEvent(studentId, firstEventId);
            await this.AssignStudentToEvent(studentId, secondEventId);

            var firstModel = new EventListViewModel()
            {
                Id = firstEventId,
                Name = "First Event",
                IsDeleted = false,
                ActivationDateAndTime = firstEventDate,
                DurationOfActivity = TimeSpan.FromMinutes(30),
                Status = Status.Pending.ToString(),
            };

            var firstModelExpectedStartDate = $"{firstEventDate.ToLocalTime().Date.ToString("dd/MM/yyyy")}";
            var firstModelExpectedDuration = $"{firstEventDate.ToLocalTime().Hour.ToString("D2")}" +
                $":{firstEventDate.ToLocalTime().Minute.ToString("D2")}" +
               $" - {firstEventDate.ToLocalTime().Add(TimeSpan.FromMinutes(30)).Hour.ToString("D2")}" +
               $":{firstEventDate.ToLocalTime().Add(TimeSpan.FromMinutes(30)).Minute.ToString("D2")}";

            var resultModelCollection = await this.Service.GetPerPageByStudentIdFilteredByStatusAsync<EventListViewModel>(
                Status.Pending, studentId, 2, 1, false);

            Assert.Equal(firstModel.Id, resultModelCollection.Last().Id);
            Assert.Equal(firstModel.Name, resultModelCollection.Last().Name);
            Assert.Equal(firstModel.IsDeleted, resultModelCollection.Last().IsDeleted);
            Assert.Equal(firstModel.ActivationDateAndTime, resultModelCollection.Last().ActivationDateAndTime);
            Assert.Equal(firstModel.DurationOfActivity, resultModelCollection.Last().DurationOfActivity);
            Assert.Equal(firstModel.Status, resultModelCollection.Last().Status);
            Assert.Equal(firstModelExpectedStartDate, resultModelCollection.Last().StartDate);
            Assert.Equal(firstModelExpectedDuration, resultModelCollection.Last().Duration);
        }

        [Fact]
        public async Task GetAllPerPageShouldReturnCorrectModelCollection()
        {
            var creatorId = Guid.NewGuid().ToString();
            var secondEventDate = DateTime.UtcNow;

            await this.CreateEventAsync("First Event", DateTime.UtcNow, creatorId);
            var secondEventId = await this.CreateEventAsync("Second Event", secondEventDate, creatorId);

            var secondModel = new EventListViewModel()
            {
                Id = secondEventId,
                Name = "Second Event",
                IsDeleted = false,
                ActivationDateAndTime = secondEventDate,
                DurationOfActivity = TimeSpan.FromMinutes(30),
                Status = Status.Pending.ToString(),
            };

            var secondModelExpectedStartDate = $"{secondEventDate.ToLocalTime().Date.ToString("dd/MM/yyyy")}";
            var secondModelExpectedDuration = $"{secondEventDate.ToLocalTime().Hour.ToString("D2")}" +
                $":{secondEventDate.ToLocalTime().Minute.ToString("D2")}" +
               $" - {secondEventDate.ToLocalTime().Add(TimeSpan.FromMinutes(30)).Hour.ToString("D2")}" +
               $":{secondEventDate.ToLocalTime().Add(TimeSpan.FromMinutes(30)).Minute.ToString("D2")}";

            var resultModelCollection = await this.Service.GetAllPerPage<EventListViewModel>(1, 1);

            Assert.Single(resultModelCollection);
            Assert.IsAssignableFrom<IList<EventListViewModel>>(resultModelCollection);

            Assert.Equal(secondModel.Id, resultModelCollection.First().Id);
            Assert.Equal(secondModel.Name, resultModelCollection.First().Name);
            Assert.Equal(secondModel.IsDeleted, resultModelCollection.First().IsDeleted);
            Assert.Equal(secondModel.ActivationDateAndTime, resultModelCollection.First().ActivationDateAndTime);
            Assert.Equal(secondModel.DurationOfActivity, resultModelCollection.First().DurationOfActivity);
            Assert.Equal(secondModel.Status, resultModelCollection.First().Status);
            Assert.Equal(secondModelExpectedDuration, resultModelCollection.First().Duration);
            Assert.Equal(secondModelExpectedStartDate, resultModelCollection.First().StartDate);
        }

        [Fact]
        public async Task GetAllPerPageShouldFilterCorrectltyByCreatorId()
        {
            var creatorId = Guid.NewGuid().ToString();
            var secondEventDate = DateTime.UtcNow;

            await this.CreateEventAsync("First Event", DateTime.UtcNow, Guid.NewGuid().ToString());
            var secondEventId = await this.CreateEventAsync("Second Event", secondEventDate, creatorId);

            var secondModel = new EventListViewModel()
            {
                Id = secondEventId,
                Name = "Second Event",
                IsDeleted = false,
                ActivationDateAndTime = secondEventDate,
                DurationOfActivity = TimeSpan.FromMinutes(30),
                Status = Status.Pending.ToString(),
            };

            var secondModelExpectedStartDate = $"{secondEventDate.ToLocalTime().Date.ToString("dd/MM/yyyy")}";
            var secondModelExpectedDuration = $"{secondEventDate.ToLocalTime().Hour.ToString("D2")}" +
                $":{secondEventDate.ToLocalTime().Minute.ToString("D2")}" +
               $" - {secondEventDate.ToLocalTime().Add(TimeSpan.FromMinutes(30)).Hour.ToString("D2")}" +
               $":{secondEventDate.ToLocalTime().Add(TimeSpan.FromMinutes(30)).Minute.ToString("D2")}";

            var resultModelCollection = await this.Service.GetAllPerPage<EventListViewModel>(1, 2, creatorId);

            Assert.Single(resultModelCollection);
            Assert.IsAssignableFrom<IList<EventListViewModel>>(resultModelCollection);

            Assert.Equal(secondModel.Id, resultModelCollection.First().Id);
            Assert.Equal(secondModel.Name, resultModelCollection.First().Name);
            Assert.Equal(secondModel.IsDeleted, resultModelCollection.First().IsDeleted);
            Assert.Equal(secondModel.ActivationDateAndTime, resultModelCollection.First().ActivationDateAndTime);
            Assert.Equal(secondModel.DurationOfActivity, resultModelCollection.First().DurationOfActivity);
            Assert.Equal(secondModel.Status, resultModelCollection.First().Status);
            Assert.Equal(secondModelExpectedDuration, resultModelCollection.First().Duration);
            Assert.Equal(secondModelExpectedStartDate, resultModelCollection.First().StartDate);
        }

        [Theory]
        [InlineData(1, 5)]
        [InlineData(1, 1000)]
        public async Task GetAllPerPageShouldTakeCorrectCountPerPage(int page, int countPerPage)
        {
            var creatorId = Guid.NewGuid().ToString();

            for (int i = 0; i < countPerPage * 2; i++)
            {
                await this.CreateEventAsync("Event", DateTime.UtcNow, creatorId);
            }

            var resultModelCollection = await this.Service.GetAllPerPage<EventListViewModel>(page, countPerPage);

            Assert.Equal(countPerPage, resultModelCollection.Count());
        }

        [Fact]
        public async Task GetAllPerPageShouldSkipCorrectly()
        {
            var creatorId = Guid.NewGuid().ToString();
            var firstEventDate = DateTime.UtcNow;
            var firstEventId = await this.CreateEventAsync("First Event", firstEventDate, creatorId);
            await this.CreateEventAsync("Second Event", DateTime.UtcNow, creatorId);

            var firstModel = new EventListViewModel()
            {
                Id = firstEventId,
                Name = "First Event",
                IsDeleted = false,
                ActivationDateAndTime = firstEventDate,
                DurationOfActivity = TimeSpan.FromMinutes(30),
                Status = Status.Pending.ToString(),
            };

            var firstModelExpectedStartDate = $"{firstEventDate.ToLocalTime().Date.ToString("dd/MM/yyyy")}";
            var firstModelExpectedDuration = $"{firstEventDate.ToLocalTime().Hour.ToString("D2")}" +
                $":{firstEventDate.ToLocalTime().Minute.ToString("D2")}" +
               $" - {firstEventDate.ToLocalTime().Add(TimeSpan.FromMinutes(30)).Hour.ToString("D2")}" +
               $":{firstEventDate.ToLocalTime().Add(TimeSpan.FromMinutes(30)).Minute.ToString("D2")}";

            var resultModelCollection = await this.Service.GetAllPerPage<EventListViewModel>(2, 1);

            Assert.Equal(firstModel.Id, resultModelCollection.Last().Id);
            Assert.Equal(firstModel.Name, resultModelCollection.Last().Name);
            Assert.Equal(firstModel.IsDeleted, resultModelCollection.Last().IsDeleted);
            Assert.Equal(firstModel.ActivationDateAndTime, resultModelCollection.Last().ActivationDateAndTime);
            Assert.Equal(firstModel.DurationOfActivity, resultModelCollection.Last().DurationOfActivity);
            Assert.Equal(firstModel.Status, resultModelCollection.Last().Status);
            Assert.Equal(firstModelExpectedStartDate, resultModelCollection.Last().StartDate);
            Assert.Equal(firstModelExpectedDuration, resultModelCollection.Last().Duration);
        }

        [Fact]
        public async Task GetEventsCountByCreatorIdAndStatusShouldReturnCorrectCount()
        {
            var creatorId = Guid.NewGuid().ToString();

            await this.CreateEventAsync("First Event", DateTime.UtcNow, creatorId);
            await this.CreateEventAsync("Second Event", DateTime.UtcNow, creatorId);

            var count = this.Service.GetEventsCountByCreatorIdAndStatus(Status.Pending, creatorId);

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
            var count = this.Service.GetEventsCountByStudentIdAndStatus(studentId, Status.Pending);

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

            await this.Service.AssignGroupsToEventAsync(groupsIds, eventId);

            var assignedGroupsIds = await this.DbContext.Events
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

            var caseWhenCreatorIdIsPassedCount = this.Service.GetAllEventsCount(firstCreatorId);
            var caseWhenNoCreatorIdIsPassedCount = this.Service.GetAllEventsCount();

            Assert.Equal(1, caseWhenCreatorIdIsPassedCount);
            Assert.Equal(2, caseWhenNoCreatorIdIsPassedCount);
        }

        [Fact]
        public async Task CreateEventAsyncShouldCreateEventCorrectly()
        {
            var creatorId = Guid.NewGuid().ToString();
            var expectedEventName = "Test Event";
            var eventActivationDate = "01/04/2020";
            var activeFrom = "08:00";
            var activeTo = "10:00";

            var expectedEventActivationDate = new DateTime(2020, 4, 1, 7, 00, 00);
            var expectedEventDuration = new TimeSpan(2, 0, 0);

            var eventId = await this.Service.CreateEventAsync(
                expectedEventName,
                eventActivationDate,
                activeFrom,
                activeTo,
                creatorId);

            var @event = await this.DbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);

            Assert.NotNull(@event);
            Assert.Equal(expectedEventName, @event.Name);
            Assert.Equal(expectedEventActivationDate, @event.ActivationDateAndTime);
            Assert.Equal(expectedEventDuration, @event.DurationOfActivity);
            Assert.Equal(Status.Pending, @event.Status);
            Assert.Equal(creatorId, @event.CreatorId);
        }

        [Fact]
        public async Task UpdateAsyncShouldUpdateEventCorrectly()
        {
            var creatorId = Guid.NewGuid().ToString();
            var eventId = await this.CreateEventAsync("Event", DateTime.UtcNow, creatorId);
            var expectedEventName = "Test Event";
            var eventActivationDate = "01/04/2020";
            var activeFrom = "08:00";
            var activeTo = "10:00";

            await this.Service.UpdateAsync(eventId, expectedEventName, eventActivationDate, activeFrom, activeTo);

            var expectedEventActivationDate = new DateTime(2020, 4, 1, 7, 00, 00);
            var expectedEventDuration = new TimeSpan(2, 0, 0);

            var updatedEvent = await this.DbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);

            Assert.Equal(expectedEventName, updatedEvent.Name);
            Assert.Equal(expectedEventActivationDate, updatedEvent.ActivationDateAndTime);
            Assert.Equal(expectedEventDuration, updatedEvent.DurationOfActivity);
        }

        [Fact]
        public async Task AssigQuizToEventAsyncShouldSetEventQuizIdCorrectly()
        {
            var creatorId = Guid.NewGuid().ToString();
            var quiz = await this.CreateQuizAsync();
            var @event = new Event
            {
                Name = "Event",
                Status = Status.Pending,
                ActivationDateAndTime = DateTime.UtcNow,
                DurationOfActivity = TimeSpan.FromMinutes(30),
                CreatorId = creatorId,
            };
            await this.DbContext.Events.AddAsync(@event);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Event>(@event).State = EntityState.Detached;

            await this.Service.AssigQuizToEventAsync(@event.Id, quiz.Id);

            var eventWithAssignedQuiz = await this.DbContext.Events.FirstOrDefaultAsync(x => x.Id == @event.Id);

            Assert.Equal(quiz.Id, eventWithAssignedQuiz.QuizId);
            Assert.Equal(quiz.Name, eventWithAssignedQuiz.QuizName);
        }

        [Fact]
        public async Task DeleteQuizFromEventAsyncShouldSetQuizIdToNull()
        {
            var creatorId = Guid.NewGuid().ToString();
            var quiz = await this.CreateQuizAsync();
            var eventId = await this.CreateEventAsync("Event", DateTime.UtcNow, creatorId, quiz.Id);
            await this.Service.DeleteQuizFromEventAsync(eventId, quiz.Id);

            var @event = await this.DbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);

            Assert.NotNull(@event);
            Assert.Null(@event.QuizId);
        }

        [Fact]
        public async Task DeleteQuizFromEventAsyncShouldChangeEventStatusToPendingIfIsActive()
        {
            var creatorId = Guid.NewGuid().ToString();
            var quiz = await this.CreateQuizAsync();
            var eventId = await this.CreateEventAsync("Event", DateTime.UtcNow, creatorId, quiz.Id);
            await this.ChangeEventStatus(eventId, Status.Active);

            await this.Service.DeleteQuizFromEventAsync(eventId, quiz.Id);

            var @event = await this.DbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);

            Assert.Equal(Status.Pending, @event.Status);
        }

        private async Task<string> CreateEventAsync(string name, DateTime activation, string creatorId, string quizId = null)
        {
            Quiz quiz;
            if (quizId != null)
            {
                quiz = await this.DbContext.Quizzes.FirstOrDefaultAsync(x => x.Id == quizId);
            }
            else
            {
                quiz = await this.CreateQuizAsync();
            }

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
            this.DbContext.Entry<Quiz>(quiz).State = EntityState.Detached;
            this.DbContext.Entry<Event>(@event).State = EntityState.Detached;
            return @event.Id;
        }

        private async Task ChangeEventStatus(string eventId, Status status)
        {
            var @event = await this.DbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            @event.Status = status;
            this.DbContext.Update(@event);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Event>(@event).State = EntityState.Detached;
        }

        private async Task AssignStudentToEvent(string studentId, string eventId)
        {
            var groupId = await this.CreateGroupAsync();
            var studentGroup = new StudentGroup() { StudentId = studentId, GroupId = groupId };
            await this.DbContext.StudentsGroups.AddAsync(studentGroup);
            await this.CreateEventGroupAsync(eventId, groupId);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<StudentGroup>(studentGroup).State = EntityState.Detached;
        }

        private async Task<string> CreateGroupAsync()
        {
            var group = new Group()
            {
                Name = "New Group",
                CreatorId = Guid.NewGuid().ToString(),
            };

            await this.DbContext.Groups.AddAsync(group);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Group>(group).State = EntityState.Detached;
            return group.Id;
        }

        private async Task<Quiz> CreateQuizAsync()
        {
            var quiz = new Quiz()
            {
                Name = "quiz",
            };

            await this.DbContext.Quizzes.AddAsync(quiz);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Quiz>(quiz).State = EntityState.Detached;
            return quiz;
        }

        private async Task<EventGroup> CreateEventGroupAsync(string eventId, string groupId)
        {
            var eventGroup = new EventGroup() { EventId = eventId, GroupId = groupId };
            await this.DbContext.EventsGroups.AddAsync(eventGroup);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<EventGroup>(eventGroup).State = EntityState.Detached;
            return eventGroup;
        }
    }
}
