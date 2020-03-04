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
    using QuizHut.Web.ViewModels.Events;
    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Students;

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

        public async Task<IList<T>> GetAllByCreatorIdAsync<T>(string id)
         => await this.repository
                .AllAsNoTracking()
                .Where(x => x.CreatorId == id)
                .OrderByDescending(x => x.CreatedOn)
                .To<T>()
                .ToListAsync();

        public async Task<GroupWithEventsViewModel> GetGroupModelAsync(string groupId, string creatorId, IList<EventsAssignViewModel> events)
        {
            var group = await this.repository.AllAsNoTracking().Where(x => x.Id == groupId).FirstOrDefaultAsync();
            var model = new GroupWithEventsViewModel()
            {
                GroupId = groupId,
                Name = group.Name,
                Events = events.Where(x => x.CreatorId == creatorId).ToList(),
            };

            return model;
        }

        public async Task<GroupDetailsViewModel> GetGroupDetailsModelAsync(string groupId)
        {
            var group = await this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == groupId)
                .Select(x => new GroupDetailsViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Events = x.EventsGroups.Select(x => new EventsAssignViewModel()
                    {
                        Name = x.Event.Name,
                        Id = x.EventId,
                    }).ToList(),
                    Students = x.StudentstGroups.Select(x => new StudentViewModel()
                    {
                        Id = x.StudentId,
                        FullName = $"{x.Student.FirstName} {x.Student.LastName}",
                        Email = x.Student.Email,
                    }).ToList(),
                })
                .FirstOrDefaultAsync();

            return group;
        }

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

        public async Task<IList<EventsAssignViewModel>> FilterEventsThatAreNotAssignedToThisGroup(string qroupId, IList<EventsAssignViewModel> events)
        {
            var assignedEventsIds = await this.eventGroupsService.GetAllEventsIdsByGroupIdAsync(qroupId);
            return events.Where(x => !assignedEventsIds.Contains(x.Id)).ToList();
        }

        public async Task<IList<StudentViewModel>> FilterStudentsThatAreNotAssignedToThisGroup(string qroupId, IList<StudentViewModel> students)
        {
            var assignedstudentsIds = await this.studentsGroupsService.GetAllStudentsIdsByGroupIdAsync(qroupId);
            return students.Where(x => !assignedstudentsIds.Contains(x.Id)).ToList();
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
