namespace QuizHut.Services.QuizzesGroups
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;

    public class QuizzesGroupsService : IQuizzesGroupsService
    {
        private readonly IRepository<QuizGroup> repository;

        public QuizzesGroupsService(IRepository<QuizGroup> repository)
        {
            this.repository = repository;
        }

        public async Task CreateAsync(string groupId, string quizId)
        {
            var quizGroup = new QuizGroup() { GroupId = groupId, QuizId = quizId };
            var quizGroupExists = await this.repository.AllAsNoTracking().Where(x => x.GroupId == groupId && x.QuizId == quizId).FirstOrDefaultAsync() != null;

            if (!quizGroupExists)
            {
                await this.repository.AddAsync(quizGroup);
                await this.repository.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(string groupId, string quizId)
        {
            var quizGroup = await this.repository
                .AllAsNoTracking()
                .Where(x => x.GroupId == groupId && x.QuizId == quizId)
                .FirstOrDefaultAsync();

            this.repository.Delete(quizGroup);
            await this.repository.SaveChangesAsync();
        }

        public async Task<string[]> GetAllQizzesIdsByGroupIdAsync(string groupId)
        => await this.repository
            .AllAsNoTracking()
            .Where(x => x.GroupId == groupId)
            .Select(x => x.QuizId)
            .ToArrayAsync();
    }
}
