namespace QuizHut.Services.Question
{
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Web.ViewModels.Question;
    using QuizHut.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public class QuestionService : IQuestionService
    {
        private readonly IDeletableEntityRepository<Question> repository;

        public QuestionService(IDeletableEntityRepository<Question> repository)
        {
            this.repository = repository;
        }

        public async Task<int> AddNewQuestionAsync(QuestionViewModel questionModel)
        {
            var question = new Question
            {
                Text = questionModel.Text,
            };

            await this.repository.AddAsync(question);
            await this.repository.SaveChangesAsync();

            return question.Id;
        }

        public async Task<int> GetQuizIdByQuestionIdAsync(int id)
        {
            var question = await this.repository.GetByIdWithDeletedAsync(id);
            return question.QuizId;
        }
    }
}
