namespace QuizHut.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data;
    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Models;
    using QuizHut.Data.Repositories;
    using QuizHut.Services.EventsGroups;
    using Xunit;

    public class EventsGroupsServiceTests
    {
        private readonly ApplicationDbContext dbContext;
        private readonly EfDeletableEntityRepository<EventGroup> eventGroupRepository;
        private readonly EventsGroupsService service;

        public EventsGroupsServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            this.dbContext = new ApplicationDbContext(options);
            this.eventGroupRepository = new EfDeletableEntityRepository<EventGroup>(this.dbContext);
            this.service = new EventsGroupsService(this.eventGroupRepository);
        }

        [Fact]
        public async Task DeleteAsyncShouldDeleteCorrectly()
        {
            var eventId = await this.CreateEventAsync();
            var groupId = await this.CreateGroupAsync();
            await this.CreateEventGroupAsync(groupId, eventId);

            await this.service.DeleteAsync(eventId, groupId);

            var eventsGroupsCount = this.dbContext.EventsGroups.Where(x => !x.IsDeleted).ToArray().Count();
            var eventGroup = await this.dbContext.EventsGroups.FindAsync(new string[] { eventId, groupId });
            Assert.Equal(0, eventsGroupsCount);
            Assert.True(eventGroup.IsDeleted);
        }

        [Fact]
        public async Task GetAllGroupsIdsByEventIdAsyncShouldReturnCorrectCollection()
        {
            var eventId = await this.CreateEventAsync();
            var firstGroupId = await this.CreateGroupAsync();
            var secondGroupId = await this.CreateGroupAsync();

            await this.CreateEventGroupAsync(firstGroupId, eventId);
            await this.CreateEventGroupAsync(secondGroupId, eventId);

            var groupIds = await this.service.GetAllGroupsIdsByEventIdAsync(eventId);

            Assert.Contains(firstGroupId, groupIds);
            Assert.Contains(secondGroupId, groupIds);
        }

        [Fact]
        public async Task CreateEventGroupAsyncShouldCreateNewEventGroupIfDoesntExistsAsDeleted()
        {
            var eventId = await this.CreateEventAsync();
            var groupId = await this.CreateGroupAsync();
            await this.service.CreateEventGroupAsync(eventId, groupId);
            var eventGroup = await this.dbContext.EventsGroups.FindAsync(new string[] { eventId, groupId });

            Assert.NotNull(eventGroup);
            Assert.Equal(eventId, eventGroup.EventId);
            Assert.Equal(groupId, eventGroup.GroupId);
        }

        [Fact]
        public async Task CreateEventGroupAsyncShouldUndeleteEventGroupIfExists()
        {
            var eventId = await this.CreateEventAsync();
            var groupId = await this.CreateGroupAsync();
            await this.CreateEventGroupAsync(groupId, eventId);
            await this.DeleteEventGroupAsync(eventId, groupId);

            await this.service.CreateEventGroupAsync(eventId, groupId);
            var eventGroup = await this.dbContext.EventsGroups.FindAsync(new string[] { eventId, groupId });
            Assert.NotNull(eventGroup);
            Assert.Equal(eventId, eventGroup.EventId);
            Assert.Equal(groupId, eventGroup.GroupId);
            Assert.False(eventGroup.IsDeleted);
        }

        private async Task CreateEventGroupAsync(string groupId, string eventId)
        {
            var eventGroup = new EventGroup()
            {
                EventId = eventId,
                GroupId = groupId,
            };

            await this.dbContext.EventsGroups.AddAsync(eventGroup);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<EventGroup>(eventGroup).State = EntityState.Detached;
        }

        private async Task<string> CreateEventAsync()
        {
            var creatorId = Guid.NewGuid().ToString();

            var @event = new Event
            {
                Name = "Name",
                Status = Status.Pending,
                ActivationDateAndTime = DateTime.UtcNow,
                DurationOfActivity = TimeSpan.FromMinutes(30),
                CreatorId = creatorId,
            };

            await this.dbContext.Events.AddAsync(@event);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<Event>(@event).State = EntityState.Detached;
            return @event.Id;
        }

        private async Task<string> CreateGroupAsync()
        {
            var creatorId = Guid.NewGuid().ToString();
            var group = new Group() { Name = "Group", CreatorId = creatorId };
            await this.dbContext.Groups.AddAsync(group);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<Group>(group).State = EntityState.Detached;
            return group.Id;
        }

        private async Task DeleteEventGroupAsync(string eventId, string groupId)
        {
            var eventGroup = await this.dbContext.EventsGroups.FirstOrDefaultAsync(x => x.EventId == eventId && x.GroupId == groupId);
            eventGroup.IsDeleted = true;
            this.dbContext.Update(eventGroup);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<EventGroup>(eventGroup).State = EntityState.Detached;
        }
    }
}
