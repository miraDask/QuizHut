namespace QuizHut.Services.Answer
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Web.ViewModels.Answer;

    public class AnswerService : IAnswerService
    {
        private readonly IDeletableEntityRepository<Answer> repository;

        public AnswerService(IDeletableEntityRepository<Answer> repository)
        {
            this.repository = repository;
        }

        public async Task<string> AddNewAnswerAsync(AnswerViewModel answerViewModel)
        {
            var answer = new Answer
            {
                Text = answerViewModel.Text,
                IsRightAnswer = answerViewModel.IsRightAnswer,
                QuestionId = answerViewModel.QuestionId,
            };

            await this.repository.AddAsync(answer);
            await this.repository.SaveChangesAsync();
            return answerViewModel.QuestionId;
        }

        public async Task UpdateAnswerAsync(string id, string text, bool isRightAnswer)
        {
            var answer = await this.repository.GetByIdWithDeletedAsync(id);
            answer.Text = text;
            answer.IsRightAnswer = isRightAnswer;

            this.repository.Update(answer);
            await this.repository.SaveChangesAsync();
        }
    }
}
