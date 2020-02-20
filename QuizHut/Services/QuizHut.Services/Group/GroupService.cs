namespace QuizHut.Services.Group
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Services.QuizGroup;
    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Quizzes;

    public class GroupService : IGroupService
    {
        private readonly IDeletableEntityRepository<Group> repository;
        private readonly IQuizGroupService quizGroupService;

        public GroupService(IDeletableEntityRepository<Group> repository, IQuizGroupService quizGroupService)
        {
            this.repository = repository;
            this.quizGroupService = quizGroupService;
        }

        public async Task AssignQuizzesToGroup(string groupId, List<string> quizzesIds)
        {
            foreach (var quizId in quizzesIds)
            {
                await this.quizGroupService.CreateAsync(groupId, quizId);
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
    }
}
