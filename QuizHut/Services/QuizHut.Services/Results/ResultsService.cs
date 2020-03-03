namespace QuizHut.Services.Results
{
    using System.Threading.Tasks;

    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;

    public class ResultsService : IResultsService
    {
        private readonly IDeletableEntityRepository<Result> repository;

        public ResultsService(IDeletableEntityRepository<Result> repository)
        {
            this.repository = repository;
        }

        public async Task<string> CreateResultAsync(string studentId, int points, int maxPoints)
        {
            var result = new Result()
            {
                Points = points,
                StudentId = studentId,
                MaxPoints = maxPoints,
            };

            await this.repository.AddAsync(result);
            await this.repository.SaveChangesAsync();
            return result.Id;
        }
    }
}
