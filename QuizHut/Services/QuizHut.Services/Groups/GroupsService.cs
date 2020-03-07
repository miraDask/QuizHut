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
        private readonly IEventsGroupsService eventGroupsService;
        private readonly IStudentsGroupsService studentsGroupsService;

        public GroupsService(
            IDeletableEntityRepository<Group> repository,
            IEventsGroupsService eventGroupsService,
            IStudentsGroupsService studentsGroupsService)
        {
            this.repository = repository;
            this.eventGroupsService = eventGroupsService;
            this.studentsGroupsService = studentsGroupsService;
        }

        public async Task AssignStudentsToGroupAsync(string groupId, IList<string> studentsIds)
        {
            foreach (var studentId in studentsIds)
            {
                await this.studentsGroupsService.CreateAsync(groupId, studentId);
            }
        }

        public async Task<string> CreateAsync(string name, string creatorId)
        {
            var group = new Group() { Name = name, CreatorId = creatorId };
            await this.repository.AddAsync(group);
            await this.repository.SaveChangesAsync();
            return group.Id;
        }

        public async Task<IList<T>> GetAllByCreatorIdAsync<T>(string id, string eventId)
        {
            var query = this.repository
                    .AllAsNoTracking();

            if (eventId != null)
            {
                var assignedGroupsIds = await this.eventGroupsService.GetAllGroupsIdsByEventIdAsync(eventId);
                query = query.Where(x => !assignedGroupsIds.Contains(x.Id));
            }

            return await query.Where(x => x.CreatorId == id)
                              .OrderByDescending(x => x.CreatedOn)
                              .To<T>()
                              .ToListAsync();
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
            await this.eventGroupsService.DeleteAsync(groupId, eventId);
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

        public async Task AssignEventsToGroupAsync(string groupId, List<string> eventsIds)
        {
            foreach (var eventId in eventsIds)
            {
                await this.eventGroupsService.CreateAsync(groupId, eventId);
            }
        }
    }
}
