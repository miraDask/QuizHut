namespace QuizHut.Services.Questions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class QuestionsService : IQuestionsService
    {
        private readonly IDeletableEntityRepository<Question> repository;
        private readonly IDeletableEntityRepository<Quiz> quizRepository;

        public QuestionsService(IDeletableEntityRepository<Question> repository, IDeletableEntityRepository<Quiz> quizRepository)
        {
            this.repository = repository;
            this.quizRepository = quizRepository;
        }

        public async Task<string> CreateQuestionAsync(string quizId, string questionText)
        {
            var quiz = await this.quizRepository.AllAsNoTracking().Select(x => new
            {
                x.Id,
                Questions = x.Questions.Count(),
            }).FirstOrDefaultAsync(x => x.Id == quizId);

            var question = new Question
            {
                Number = quiz.Questions + 1,
                Text = questionText,
                QuizId = quizId,
            };

            await this.repository.AddAsync(question);
            await this.repository.SaveChangesAsync();

            return question.Id;
        }

        public async Task DeleteQuestionByIdAsync(string id)
        {
            var question = await this.repository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            this.repository.Delete(question);
            await this.repository.SaveChangesAsync();
            await this.UpdateAllQuestionsInQuizNumeration(question.QuizId);
        }

        public async Task UpdateAllQuestionsInQuizNumeration(string quizId)
        {
            var count = 0;

            var questions = this.repository
              .AllAsNoTracking()
              .Where(x => x.QuizId == quizId)
              .OrderBy(x => x.Number);

            foreach (var question in questions)
            {
                question.Number = ++count;
                this.repository.Update(question);
            }

            await this.repository.SaveChangesAsync();
        }

        public async Task Update(string id, string text)
        {
            var question = await this.repository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            question.Text = text;
            this.repository.Update(question);
            await this.repository.SaveChangesAsync();
        }

        public async Task<T> GetByIdAsync<T>(string id)
        => await this.repository.AllAsNoTracking()
            .Where(x => x.Id == id)
            .To<T>()
            .FirstOrDefaultAsync();

        public async Task<IList<T>> GetAllByQuizIdAsync<T>(string id)
        => await this.repository.AllAsNoTracking()
            .Where(x => x.QuizId == id)
            .OrderBy(x => x.Number)
            .To<T>()
            .ToListAsync();

        public async Task<int> GetAllByQuizIdCountAsync(string id)
        => await this.repository.AllAsNoTracking().Where(x => x.QuizId == id).CountAsync();

        public async Task<T> GetQuestionByQuizIdAndNumberAsync<T>(string quizId, int number)
        => await this.repository.AllAsNoTracking()
            .Where(x => x.QuizId == quizId && x.Number == number)
            .To<T>()
            .FirstOrDefaultAsync();
    }
}
