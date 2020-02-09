namespace QuizHut.Services.Answer
{
    using System.Threading.Tasks;

    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Common;
    using QuizHut.Web.ViewModels.Answer;

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
