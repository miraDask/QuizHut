namespace QuizHut.Services.Quizzes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class QuizzesService : IQuizzesService
    {
        private readonly IDeletableEntityRepository<Quiz> repository;

        public QuizzesService(IDeletableEntityRepository<Quiz> repository)
        {
            this.repository = repository;
        }

        public async Task<string> AddNewQuizAsync(string name, string description, int? timer, string creatorId, string password)
        {
            var quiz = new Quiz
            {
                Name = name,
                Description = description,
                Timer = timer,
                CreatorId = creatorId,
                Password = password,
            };

            await this.repository.AddAsync(quiz);
            await this.repository.SaveChangesAsync();

            return quiz.Id;
        }

        public async Task<IList<T>> GetAllByCreatorIdAsync<T>(string id)
          => await this.repository
                 .AllAsNoTracking()
                 .Where(x => x.CreatorId == id)
                 .OrderByDescending(x => x.CreatedOn)
                 .To<T>()
                 .ToListAsync();

        public async Task<T> GetQuizByIdAsync<T>(string id)
       => await this.repository
               .AllAsNoTracking()
               .Where(x => x.Id == id)
               .To<T>()
               .FirstOrDefaultAsync();

        public async Task<IList<T>> GetAllAsync<T>()
         => await this.repository
                .AllAsNoTracking()
                .To<T>()
                .ToListAsync();

        public async Task DeleteByIdAsync(string id)
        {
            var quiz = await this.repository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            this.repository.Delete(quiz);
            await this.repository.SaveChangesAsync();
        }

        public async Task UpdateAsync(string id, string name, string description, int? timer, string password)
        {
            var quiz = await this.repository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            quiz.Name = name;
            quiz.Description = description;
            quiz.Timer = timer;
            quiz.Password = password;

            this.repository.Update(quiz);
            await this.repository.SaveChangesAsync();
        }

        public async Task<string> GetQuizIdByPasswordAsync(string password)
        => await this.repository.AllAsNoTracking()
            .Where(x => x.Password == password)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        public async Task<string> GetIdByPassword(string password)
       => await this.repository
            .AllAsNoTrackingWithDeleted()
            .Where(x => x.Password == password)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();
    }
}
