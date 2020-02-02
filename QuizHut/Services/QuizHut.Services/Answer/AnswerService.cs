namespace QuizHut.Services.Answer
{
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Web.ViewModels.Question;
    using QuizHut.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using QuizHut.Web.ViewModels.Answer;

    public class AnswerService : IAnswerService
    {
        private readonly IDeletableEntityRepository<Answer> repository;

        public AnswerService(IDeletableEntityRepository<Answer> repository)
        {
            this.repository = repository;
        }

        public async Task AddNewAnswerAsync(AnswerViewModel answerViewModel)
        {
            var answer = new Answer
            {
                Text = answerViewModel.Text,
                IsRightAnswer = answerViewModel.IsRightAnswer,
            };

            await this.repository.AddAsync(answer);
            await this.repository.SaveChangesAsync();
        }
    }
}
