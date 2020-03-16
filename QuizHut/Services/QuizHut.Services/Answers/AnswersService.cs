namespace QuizHut.Services.Answers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class AnswersService : IAnswersService
    {
        private readonly IDeletableEntityRepository<Answer> repository;

        public AnswersService(IDeletableEntityRepository<Answer> repository)
        {
            this.repository = repository;
        }

        public async Task CreateAnswerAsync(string answerText, bool isRightAnswer, string questionId)
        {
            var answer = new Answer
            {
                Text = answerText,
                IsRightAnswer = isRightAnswer,
                QuestionId = questionId,
            };

            await this.repository.AddAsync(answer);
            await this.repository.SaveChangesAsync();
        }

        public async Task<T> GetByIdAsync<T>(string id)
        => await this.repository
            .AllAsNoTracking()
            .Where(x => x.Id == id)
            .To<T>()
            .FirstOrDefaultAsync();

        public async Task UpdateAsync(string id, string text, bool isRightAnswer)
        {
            var answer = await this.repository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            answer.Text = text;
            answer.IsRightAnswer = isRightAnswer;

            this.repository.Update(answer);
            await this.repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var answer = await this.repository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            this.repository.Delete(answer);
            await this.repository.SaveChangesAsync();
        }
    }
}
