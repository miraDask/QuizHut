namespace QuizHut.Services.Groups
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Common;
    using QuizHut.Services.EventsGroups;
    using QuizHut.Services.Mapping;
    using QuizHut.Services.StudentsGroups;
    using QuizHut.Services.Tools.Expressions;

    public class GroupsService : IGroupsService
    {
        private readonly IDeletableEntityRepository<Group> repository;
        private readonly IStudentsGroupsService studentsGroupsService;
        private readonly IExpressionBuilder expressionBuilder;
        private readonly IEventsGroupsService eventsGroupsService;

        public GroupsService(
            IDeletableEntityRepository<Group> repository,
            IStudentsGroupsService studentsGroupsService,
            IExpressionBuilder expressionBuilder,
            IEventsGroupsService eventsGroupsService)
        {
            this.repository = repository;
            this.studentsGroupsService = studentsGroupsService;
            this.expressionBuilder = expressionBuilder;
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

        public async Task<IList<T>> GetAllAsync<T>(string creatorId = null, string eventId = null)
        {
            var query = this.repository.AllAsNoTracking();

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            if (eventId != null)
            {
                query = query.Where(x => !x.EventsGroups.Any(x => x.EventId == eventId));
            }

            return await query.OrderByDescending(x => x.CreatedOn).To<T>().ToListAsync();
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

        public async Task<IList<T>> GetAllPerPageAsync<T>(
            int page,
            int countPerPage,
            string creatorId = null,
            string searchCriteria = null,
            string searchText = null)
        {
            var query = this.repository.AllAsNoTracking();

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            var emptyNameInput = searchText == null && searchCriteria == ServicesConstants.Name;
            if (searchCriteria != null && !emptyNameInput)
            {
                var filter = this.expressionBuilder.GetExpression<Group>(searchCriteria, searchText);
                query = query.Where(filter);
            }

            return await query
                   .OrderByDescending(x => x.CreatedOn)
                   .Skip(countPerPage * (page - 1))
                   .Take(countPerPage)
                   .To<T>()
                   .ToListAsync();
        }

        public async Task<int> GetAllGroupsCountAsync(string creatorId = null, string searchCriteria = null, string searchText = null)
        {
            var query = this.repository.AllAsNoTracking();

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            var emptyNameInput = searchText == null && searchCriteria == ServicesConstants.Name;
            if (searchCriteria != null && !emptyNameInput)
            {
                var filter = this.expressionBuilder.GetExpression<Group>(searchCriteria, searchText);
                query = query.Where(filter);
            }

            return await query.CountAsync();
        }

        public async Task<T> GetEventsFirstGroupAsync<T>(string eventId)
        => await this.repository
            .AllAsNoTracking()
            .Where(x => x.EventsGroups.Any(x => x.EventId == eventId))
            .To<T>()
            .FirstOrDefaultAsync();
    }
}
