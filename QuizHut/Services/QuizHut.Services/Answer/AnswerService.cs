namespace QuizHut.Services.Answer
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Web.ViewModels.Answers;

    public class AnswerService : IAnswerService
    {
        private readonly IDeletableEntityRepository<Answer> repository;

        public AnswerService(IDeletableEntityRepository<Answer> repository)
        {
            this.repository = repository;
        }

        public async Task AddNewAnswerAsync(string answerText, bool isRightAnswer, string questionId)
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

        public async Task<AnswerViewModel> GetAnswerModelAsync(string id)
        => await this.repository
            .AllAsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AnswerViewModel()
            {
                Id = x.Id,
                Text = x.Text,
                IsRightAnswer = x.IsRightAnswer,
            }).FirstOrDefaultAsync();

        public async Task UpdateAsync(string id, string text, bool isRightAnswer)
        {
            var answer = await this.repository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            answer.Text = text;
            answer.IsRightAnswer = isRightAnswer;

            this.repository.Update(answer);
            await this.repository.SaveChangesAsync();
        }

        public async Task Delete(string id)
        {
            var answer = await this.repository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            this.repository.Delete(answer);
            await this.repository.SaveChangesAsync();
        }

        //public async Task UpdateAnswerAsync(string id, string text, bool isRightAnswer)
        //{
        //    var answer = await this.repository.GetByIdWithDeletedAsync(id);
        //    answer.Text = text;
        //    answer.IsRightAnswer = isRightAnswer;

        //    this.repository.Update(answer);
        //    await this.repository.SaveChangesAsync();
        //}
    }
}
