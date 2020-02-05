namespace QuizHut.Services.Question
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Web.ViewModels.Question;

    public class QuestionService : IQuestionService
    {
        private readonly IDeletableEntityRepository<Question> repository;

        public QuestionService(IDeletableEntityRepository<Question> repository)
        {
            this.repository = repository;
        }

        public async Task<string> AddNewQuestionAsync(QuestionViewModel questionModel)
        {
            var question = new Question
            {
                Text = questionModel.Text,
                QuizId = questionModel.QuizId,
            };

            await this.repository.AddAsync(question);
            await this.repository.SaveChangesAsync();

            return question.Id;
        }

        public async Task<string> GetQuizIdByQuestionIdAsync(string id)
        {
            var question = await this.repository.GetByIdWithDeletedAsync(id);
            return question.QuizId;
        }
    }
}
