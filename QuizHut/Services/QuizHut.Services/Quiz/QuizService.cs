namespace QuizHut.Services.Quiz
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Quizzes;

    public class QuizService : IQuizService
    {
        private readonly IDeletableEntityRepository<Quiz> repository;

        public QuizService(IDeletableEntityRepository<Quiz> repository)
        {
            this.repository = repository;
        }

        public async Task<string> AddNewQuizAsync(string name, string description, string activationDate, int? duration, string creatorId, string password)
        {
            DateTime? date = null;

            var quiz = new Quiz
            {
                Name = name,
                Description = description,
                ActivationDateAndTime = activationDate == null ? date : DateTime.Parse(activationDate),
                Duration = duration,
                CreatorId = creatorId,
                Password = password,
            };

            await this.repository.AddAsync(quiz);
            await this.repository.SaveChangesAsync();

            return quiz.Id;
        }

        public async Task<T> GetQuizByIdAsync<T>(string id)
       => await this.repository
               .AllAsNoTracking()
               .Where(x => x.Id == id)
               .To<T>()
               .FirstOrDefaultAsync();

        public async Task<IEnumerable<T>> GetAllAsync<T>()
         => await this.repository
                .AllAsNoTracking()
                .Where(x => !x.IsDeleted)
                .To<T>()
                .ToListAsync();

        public async Task DeleteByIdAsync(string id)
        {
            var quiz = await this.repository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            this.repository.Delete(quiz);
            await this.repository.SaveChangesAsync();
        }

        public async Task UpdateAsync(string id, string name, string description, string activationDate, int? duration, string password)
        {
            DateTime? date = null;

            var quiz = await this.repository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            quiz.Name = name;
            quiz.Description = description;
            quiz.ActivationDateAndTime = activationDate == null ? date : DateTime.Parse(activationDate);
            quiz.Duration = duration;
            quiz.Password = password;

            this.repository.Update(quiz);
            await this.repository.SaveChangesAsync();
        }

        public async Task<string> GetQuizIdByPasswordAsync(string password)
        => await this.repository.AllAsNoTracking()
            .Where(x => x.Password == password)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();
    }
}
