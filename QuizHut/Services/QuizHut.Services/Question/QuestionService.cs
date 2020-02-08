namespace QuizHut.Services.Question
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Common;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Answer;
    using QuizHut.Web.ViewModels.Question;

    public class QuestionService : IQuestionService
    {
        private readonly IDeletableEntityRepository<Question> repository;
        private readonly IDeletableEntityRepository<Quiz> quizRepository;

        public QuestionService(IDeletableEntityRepository<Question> repository, IDeletableEntityRepository<Quiz> quizRepository)
        {
            this.repository = repository;
            this.quizRepository = quizRepository;
        }

        public async Task<string> AddNewQuestionAsync(QuestionViewModel questionModel)
        {
            var quizId = questionModel.QuizId;
            var quiz = await this.quizRepository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == quizId);

            var question = new Question
            {
                Number = quiz.Questions.Count,
                Text = questionModel.Text,
                QuizId = quizId,
            };

            await this.repository.AddAsync(question);
            await this.repository.SaveChangesAsync();

            return question.Id;
        }

        public IOrderedQueryable<QuestionViewModel> GetAllQuestionsQuizById(string id)
        {
            var questions = this.repository
               .AllAsNoTracking()
               .Where(x => x.QuizId == id)
               .OrderBy(x => x.Number)
               .ToList();

            var models = questions.Select(x => new QuestionViewModel
            {
                Text = x.Text,
                Answers = x.Answers.Select(y => new AnswerViewModel
                {
                    Text = y.Text,
                }).ToList(),
            }).AsQueryable().OrderBy(x => x);

            return models;
        }

        //public async Task<string> GetQuizIdByQuestionIdAsync(string id)
        //{
        //    var question = await this.repository.GetByIdWithDeletedAsync(id);
        //    return question.QuizId;
        //}
    }
}
