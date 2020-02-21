namespace QuizHut.Services.QuizGroup
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;

    public class QuizGroupService : IQuizGroupService
    {
        private readonly IRepository<QuizGroup> repository;

        public QuizGroupService(IRepository<QuizGroup> repository)
        {
            this.repository = repository;
        }

        public async Task CreateAsync(string groupId, string quizId)
        {
            var quizGroup = new QuizGroup() { GroupId = groupId, QuizId = quizId };
            await this.repository.AddAsync(quizGroup);
            await this.repository.SaveChangesAsync();
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
