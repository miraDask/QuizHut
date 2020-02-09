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

    public class QuizService : IQuizService
    {
        private readonly IDeletableEntityRepository<Quiz> repository;

        public QuizService(IDeletableEntityRepository<Quiz> repository)
        {
            this.repository = repository;
        }

        public async Task<string> AddNewQuizAsync(string name, string description, string activationDate, int? duration, string creatorId)
        {
            DateTime? date = null;

            var quiz = new Quiz
            {
                Name = name,
                Description = description,
                ActivationDateAndTime = activationDate == null ? date : DateTime.Parse(activationDate),
                Duration = duration,
                CreatorId = creatorId,
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
                .To<T>()
                .ToListAsync();
    }
}
