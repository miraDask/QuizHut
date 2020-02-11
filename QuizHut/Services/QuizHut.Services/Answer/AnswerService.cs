namespace QuizHut.Services.Answer
{
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;

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

        public async Task<string> GetAnswerId(string questionId, string answerText)
        {
            var answer = await this.repository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.QuestionId == questionId && x.Text == answerText);

            if (answer == null)
            {
                 answer = await this.repository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == questionId && x.Text == answerText);
            }

            return answer.Id;
        }

        public async Task UpdateAsync(string id, string text, bool isRightAnswer)
        {
            var answer = await this.repository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            answer.Text = text;
            answer.IsRightAnswer = isRightAnswer;

            this.repository.Update(answer);
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
