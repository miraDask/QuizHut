namespace QuizHut.Services.EventsGroups
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;

    public class EventsGroupsService : IEventsGroupsService
    {
        private readonly IRepository<EventGroup> repository;

        public EventsGroupsService(IRepository<EventGroup> repository)
        {
            this.repository = repository;
        }

        public async Task CreateAsync(string groupId, string eventId)
        {
            var eventGroup = new EventGroup() { GroupId = groupId, EventId = eventId };
            var eventGroupExists = await this.repository.AllAsNoTracking().Where(x => x.GroupId == groupId && x.EventId == eventId).FirstOrDefaultAsync() != null;

            if (!eventGroupExists)
            {
                await this.repository.AddAsync(eventGroup);
                await this.repository.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(string groupId, string eventId)
        {
            var eventGroup = await this.repository
                .AllAsNoTracking()
                .Where(x => x.GroupId == groupId && x.EventId == eventId)
                .FirstOrDefaultAsync();

            this.repository.Delete(eventGroup);
            await this.repository.SaveChangesAsync();
        }

        public async Task<string[]> GetAllEventsIdsByGroupIdAsync(string groupId)
        => await this.repository
            .AllAsNoTracking()
            .Where(x => x.GroupId == groupId)
            .Select(x => x.EventId)
            .ToArrayAsync();

        public async Task<string[]> GetAllGroupsIdsByEventIdAsync(string eventId)
        => await this.repository
            .AllAsNoTracking()
            .Where(x => x.EventId == eventId)
            .Select(x => x.GroupId)
            .ToArrayAsync();
    }
}
