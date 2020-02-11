namespace QuizHut.Services.QuizResult
{
    using System.Threading.Tasks;

    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;

    public class QuizResultService : IQuizResultService
    {
        private readonly IDeletableEntityRepository<QuizResult> repository;

        public QuizResultService(IDeletableEntityRepository<QuizResult> repository)
        {
            this.repository = repository;
        }

        public async Task CreateQuizResultAsync(string participantId, int points, int maxPoints, string quizId)
        {
            var quizResult = new QuizResult()
            {
                QuizId = quizId,
                ParticipantId = participantId,
                Points = points,
                MaxPoints = maxPoints,
            };

            await this.repository.AddAsync(quizResult);
            await this.repository.SaveChangesAsync();
        }
    }
}
