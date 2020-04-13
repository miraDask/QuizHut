namespace QuizHut.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Models;
    using QuizHut.Services.EventsGroups;
    using Xunit;

    public class EventsGroupsServiceTests : BaseServiceTests
    {
        private IEventsGroupsService Service => this.ServiceProvider.GetRequiredService<IEventsGroupsService>();

        [Fact]
        public async Task DeleteAsyncShouldDeleteCorrectly()
        {
            var eventId = await this.CreateEventAsync();
            var groupId = await this.CreateGroupAsync();
            await this.CreateEventGroupAsync(groupId, eventId);

            await this.Service.DeleteAsync(eventId, groupId);

            var eventsGroupsCount = this.DbContext.EventsGroups.Where(x => !x.IsDeleted).ToArray().Count();
            var eventGroup = await this.DbContext.EventsGroups.FindAsync(new string[] { eventId, groupId });
            Assert.Equal(0, eventsGroupsCount);
            Assert.True(eventGroup.IsDeleted);
        }

        [Fact]
        public async Task CreateEventGroupAsyncShouldCreateNewEventGroupIfDoesntExistsAsDeleted()
        {
            var eventId = await this.CreateEventAsync();
            var groupId = await this.CreateGroupAsync();
            await this.Service.CreateEventGroupAsync(eventId, groupId);
            var eventGroup = await this.DbContext.EventsGroups.FindAsync(new string[] { eventId, groupId });

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

            await this.Service.CreateEventGroupAsync(eventId, groupId);
            var eventGroup = await this.DbContext.EventsGroups.FindAsync(new string[] { eventId, groupId });
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

            await this.DbContext.EventsGroups.AddAsync(eventGroup);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<EventGroup>(eventGroup).State = EntityState.Detached;
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

            await this.DbContext.Events.AddAsync(@event);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Event>(@event).State = EntityState.Detached;
            return @event.Id;
        }

        private async Task<string> CreateGroupAsync()
        {
            var creatorId = Guid.NewGuid().ToString();
            var group = new Group() { Name = "Group", CreatorId = creatorId };
            await this.DbContext.Groups.AddAsync(group);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Group>(group).State = EntityState.Detached;
            return group.Id;
        }

        private async Task DeleteEventGroupAsync(string eventId, string groupId)
        {
            var eventGroup = await this.DbContext.EventsGroups.FirstOrDefaultAsync(x => x.EventId == eventId && x.GroupId == groupId);
            eventGroup.IsDeleted = true;
            this.DbContext.Update(eventGroup);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<EventGroup>(eventGroup).State = EntityState.Detached;
        }
    }
}
