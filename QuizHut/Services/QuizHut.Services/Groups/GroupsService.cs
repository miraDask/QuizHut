namespace QuizHut.Services.Groups
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Services.StudentsGroups;

    public class GroupsService : IGroupsService
    {
        private readonly IDeletableEntityRepository<Group> repository;
        private readonly IDeletableEntityRepository<Event> eventRepository;
        private readonly IStudentsGroupsService studentsGroupsService;

        public GroupsService(
            IDeletableEntityRepository<Group> repository,
            IDeletableEntityRepository<Event> eventRepository,
            IStudentsGroupsService studentsGroupsService)
        {
            this.repository = repository;
            this.eventRepository = eventRepository;
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

        public async Task<IList<T>> GetAllByCreatorIdAsync<T>(string id)
        => await this.repository
                    .AllAsNoTracking()
                    .OrderByDescending(x => x.CreatedOn)
                    .Where(x => x.CreatorId == id)
                    .To<T>()
                    .ToListAsync();

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
            var group = await this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == groupId)
                .FirstOrDefaultAsync();
            group.Events = group.Events.Where(x => x.Id != eventId).ToList();
            var @event = await this.eventRepository.
                AllAsNoTracking()
                .Where(x => x.Id == eventId)
                .FirstOrDefaultAsync();
            @event.GroupId = null;

            this.eventRepository.Update(@event);
            this.repository.Update(group);

            await this.eventRepository.SaveChangesAsync();
            await this.repository.SaveChangesAsync();
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
            var group = await this.repository
                 .AllAsNoTracking()
                 .Where(x => x.Id == groupId)
                 .FirstOrDefaultAsync();

            foreach (var eventId in evenstIds)
            {
                var @event = await this.eventRepository
                    .AllAsNoTracking()
                    .Where(x => x.Id == eventId)
                    .FirstOrDefaultAsync();

                group.Events.Add(@event);
                @event.Group = group;
                this.repository.Update(group);
                this.eventRepository.Update(@event);
            }

            await this.repository.SaveChangesAsync();
            await this.eventRepository.SaveChangesAsync();
        }

        public async Task<T> GetGroupModelByEventIdAsync<T>(string eventId)
        => await this.repository
            .AllAsNoTracking()
            .Where(x => x.Events.Any(x => x.Id == eventId))
            .To<T>()
            .FirstOrDefaultAsync();
    }
}
