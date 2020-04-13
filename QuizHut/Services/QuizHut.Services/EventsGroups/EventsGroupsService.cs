namespace QuizHut.Services.EventsGroups
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;

    public class EventsGroupsService : IEventsGroupsService
    {
        private readonly IDeletableEntityRepository<EventGroup> repository;

        public EventsGroupsService(IDeletableEntityRepository<EventGroup> repository)
        {
            this.repository = repository;
        }

        public async Task CreateEventGroupAsync(string eventId, string groupId)
        {
            var deletedEventGroup = await this.repository
                .AllAsNoTrackingWithDeleted()
                .Where(x => x.GroupId == groupId && x.EventId == eventId)
                .FirstOrDefaultAsync();

            if (deletedEventGroup != null)
            {
                this.repository.Undelete(deletedEventGroup);
            }
            else
            {
                var eventGroup = new EventGroup() { EventId = eventId, GroupId = groupId };
                await this.repository.AddAsync(eventGroup);
            }

            await this.repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(string eventId, string groupId)
        {
            var eventGroup = await this.repository
                .AllAsNoTracking()
                .Where(x => x.EventId == eventId && x.GroupId == groupId)
                .FirstOrDefaultAsync();

            this.repository.Delete(eventGroup);
            await this.repository.SaveChangesAsync();
        }
    }
}
