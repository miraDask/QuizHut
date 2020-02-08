namespace QuizHut.Services.Quiz
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Common;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Quiz;

    public class QuizService : IQuizService
    {
        private readonly IDeletableEntityRepository<Quiz> repository;

        public QuizService(IDeletableEntityRepository<Quiz> repository)
        {
            this.repository = repository;
        }

        public async Task<string> AddNewQuizAsync(InputQuizViewModel quizModel)
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

        //public bool GetDublicatedQuizPassword(string password)
        //{
        //    return this.repository.AllAsNoTracking().Select(x => x.Password).Any(x => x == password);
        //}

        public async Task<T> GetQuizByIdAsync<T>(string id)
       => await this.repository
               .AllAsNoTracking()
               .Where(x => x.Id == id)
               .To<T>()
               .FirstOrDefaultAsync();

        public async Task<IEnumerable<T>> GetAllAsync<T>()
         => await this.repository
                .AllAsNoTracking()
                .To<T>()
                .ToListAsync();
    }
}
