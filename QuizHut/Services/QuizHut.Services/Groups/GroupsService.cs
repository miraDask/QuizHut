namespace QuizHut.Services.Groups
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Services.QuizzesGroups;
    using QuizHut.Services.StudentsGroups;
    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Quizzes;
    using QuizHut.Web.ViewModels.Students;

    public class GroupsService : IGroupsService
    {
        private readonly IDeletableEntityRepository<Group> repository;
        private readonly IQuizzesGroupsService quizzesGroupsService;
        private readonly IStudentsGroupsService studentsGroupsService;

        public GroupsService(
            IDeletableEntityRepository<Group> repository,
            IQuizzesGroupsService quizzesGroupsService,
            IStudentsGroupsService studentsGroupsService)
        {
            this.repository = repository;
            this.quizzesGroupsService = quizzesGroupsService;
            this.studentsGroupsService = studentsGroupsService;
        }

        public async Task AssignQuizzesToGroupAsync(string groupId, List<string> quizzesIds)
        {
            foreach (var quizId in quizzesIds)
            {
                await this.quizzesGroupsService.CreateAsync(groupId, quizId);
            }
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

        public async Task<GroupWithQuizzesViewModel> GetGroupModelAsync(string groupId, string creatorId, IList<QuizAssignViewModel> quizzes)
        {
            var group = await this.repository.AllAsNoTracking().Where(x => x.Id == groupId).FirstOrDefaultAsync();
            var model = new GroupWithQuizzesViewModel()
            {
                GroupId = groupId,
                Name = group.Name,
                Quizzes = quizzes.Where(x => x.CreatorId == creatorId).ToList(),
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
                    Quizzes = x.QuizzesGroups.Select(x => new QuizAssignViewModel()
                    {
                        Name = x.Quiz.Name,
                        Id = x.QuizId,
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

        public async Task DeleteQuizFromGroupAsync(string groupId, string quizId)
        {
            await this.quizzesGroupsService.DeleteAsync(groupId, quizId);
        }

        public async Task DeleteStudentFromGroupAsync(string groupId, string studentId)
        {
            await this.studentsGroupsService.DeleteAsync(groupId, studentId);
        }

        public async Task<IList<QuizAssignViewModel>> FilterQuizzesThatAreNotAssignedToThisGroup(string qroupId, IList<QuizAssignViewModel> quizzes)
        {
            var assignedQuizzesIds = await this.quizzesGroupsService.GetAllQizzesIdsByGroupIdAsync(qroupId);
            return quizzes.Where(x => !assignedQuizzesIds.Contains(x.Id)).ToList();
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
    }
}
