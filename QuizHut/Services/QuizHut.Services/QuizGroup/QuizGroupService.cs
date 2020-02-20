namespace QuizHut.Services.QuizGroup
{
    using System.Threading.Tasks;

    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;

    public class QuizGroupService : IQuizGroupService
    {
        private readonly IDeletableEntityRepository<QuizGroup> repository;

        public QuizGroupService(IDeletableEntityRepository<QuizGroup> repository)
        {
            this.repository = repository;
        }

        public async Task CreateAsync(string groupId, string quizId)
        {
            var quizGroup = new QuizGroup() { GroupId = groupId, QuizId = quizId };
            await this.repository.AddAsync(quizGroup);
            await this.repository.SaveChangesAsync();
        }
    }
}
