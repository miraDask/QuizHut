namespace QuizHut.Services.Groups
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.EventsGroups;
    using QuizHut.Services.Mapping;
    using QuizHut.Services.StudentsGroups;

    public class GroupsService : IGroupsService
    {
        private readonly IDeletableEntityRepository<Group> repository;
        private readonly IStudentsGroupsService studentsGroupsService;
        private readonly IEventsGroupsService eventsGroupsService;

        public GroupsService(
            IDeletableEntityRepository<Group> repository,
            IStudentsGroupsService studentsGroupsService,
            IEventsGroupsService eventsGroupsService)
        {
            this.repository = repository;
            this.studentsGroupsService = studentsGroupsService;
            this.eventsGroupsService = eventsGroupsService;
        }

        public async Task AssignStudentsToGroupAsync(string groupId, IList<string> studentsIds)
        {
            foreach (var studentId in studentsIds)
            {
                await this.studentsGroupsService.CreateStudentGroupAsync(groupId, studentId);
            }
        }

        public async Task<string> CreateGroupAsync(string name, string creatorId)
        {
            var group = new Group() { Name = name, CreatorId = creatorId };
            await this.repository.AddAsync(group);
            await this.repository.SaveChangesAsync();
            return group.Id;
        }

        public async Task<IList<T>> GetAllByCreatorIdAsync<T>(string id, string eventId = null)
        {
            var query = this.repository
                    .AllAsNoTracking()
                    .OrderByDescending(x => x.CreatedOn)
                    .Where(x => x.CreatorId == id);
            if (eventId != null)
            {
                var assignedGroupsIds = await this.eventsGroupsService.GetAllGroupsIdsByEventIdAsync(eventId);
                query = query.Where(x => !assignedGroupsIds.Contains(x.Id));
            }

            return await query.To<T>().ToListAsync();
        }

        public async Task<T> GetGroupModelAsync<T>(string groupId)
         => await this.repository
            .AllAsNoTracking()
            .Where(x => x.Id == groupId)
            .To<T>()
            .FirstOrDefaultAsync();

        public async Task DeleteAsync(string groupId)
        {
            var group = await this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == groupId)
                .FirstOrDefaultAsync();
            this.repository.Delete(group);
            await this.repository.SaveChangesAsync();
        }

        public async Task DeleteEventFromGroupAsync(string groupId, string eventId)
        {
            await this.eventsGroupsService.DeleteAsync(eventId, groupId);
        }

        public async Task DeleteStudentFromGroupAsync(string groupId, string studentId)
        {
            await this.studentsGroupsService.DeleteAsync(groupId, studentId);
        }

        public async Task UpdateNameAsync(string groupId, string newName)
        {
            var group = await this.repository.AllAsNoTracking().Where(x => x.Id == groupId).FirstOrDefaultAsync();
            group.Name = newName;
            this.repository.Update(group);
            await this.repository.SaveChangesAsync();
        }

        public async Task AssignEventsToGroupAsync(string groupId, IList<string> evenstIds)
        {
            foreach (var eventId in evenstIds)
            {
                await this.eventsGroupsService.CreateEventGroupAsync(eventId, groupId);
            }
        }

        public async Task<IEnumerable<T>> GetAllByEventIdAsync<T>(string eventId)
        => await this.repository
            .AllAsNoTracking()
            .Where(x => x.EventsGroups.Any(x => x.EventId == eventId))
            .To<T>()
            .ToListAsync();

        public async Task<IList<T>> GetAllPerPage<T>(int page, int countPerPage, string creatorId = null)
        {
            var query = this.repository.AllAsNoTracking();

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            return await query
                   .OrderByDescending(x => x.CreatedOn)
                   .Skip(countPerPage * (page - 1))
                   .Take(countPerPage)
                   .To<T>()
                   .ToListAsync();
        }

        public int GetAllGroupsCount(string creatorId = null)
        {
            var query = this.repository.AllAsNoTracking();

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            return query.Count();
        }
    }
}
