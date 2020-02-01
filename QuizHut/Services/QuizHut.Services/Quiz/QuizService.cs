namespace QuizHut.Services.Quiz
{
    
    using System.Threading.Tasks;
    using QuizHut.Services.Mapping;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Web.ViewModels.Quiz;
    using System;
    using Microsoft.AspNetCore.Identity;
    using System.Linq;

    public class QuizService : IQuizService
    {
        private readonly IDeletableEntityRepository<Quiz> repository;

        public QuizService(IDeletableEntityRepository<Quiz> repository)
        {
            this.repository = repository;
        }

        public async Task<int> AddNewQuizAsync(InputQuizViewModel quizModel)
        {
            var quiz = new Quiz
            {
                Name = quizModel.Name,
                Description = quizModel.Description,
                ActivationDateAndTime = Convert.ToDateTime(quizModel.ActivationDate),
                Duration = quizModel.Duration,
                CreatorId = quizModel.CreatorId,
            };

            await this.repository.AddAsync(quiz);
            await this.repository.SaveChangesAsync();

            return quiz.Id;
        }
    }
}
