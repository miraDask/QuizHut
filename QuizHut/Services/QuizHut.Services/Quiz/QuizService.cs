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

        public async Task<string> AddNewQuizAsync(string name, string description, string activationDate, string activeFrom, string activeTo, int? timer, string creatorId, string password)
        {
            var quiz = new Quiz
            {
                Name = name,
                Description = description,
                ActivationDateAndTime = this.GetActivationDateAndTime(activationDate, activeFrom),
                DurationOfActivity = this.GetDurationOfActivity(activationDate, activeFrom, activeTo),
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

        public async Task UpdateAsync(string id, string name, string description, string activationDate, string activeFrom, string activeTo, int? timer, string password)
        {

            var quiz = await this.repository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            quiz.Name = name;
            quiz.Description = description;
            quiz.ActivationDateAndTime = this.GetActivationDateAndTime(activationDate, activeFrom);
            quiz.DurationOfActivity = this.GetDurationOfActivity(activationDate, activeFrom, activeTo);
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

        private DateTime? GetActivationDateAndTime(string activationDate, string activeFrom)
        {
            DateTime? nullableDate = null;

            if (activationDate == null)
            {
                return nullableDate;
            }

            return activeFrom == null
                ? DateTime.Parse(activationDate) : DateTime.Parse(activationDate).Add(TimeSpan.Parse(activeFrom));
        }

        private TimeSpan? GetDurationOfActivity(string activationDate, string activeFrom, string activeTo)
        {
            TimeSpan? nulllableTimeSpan = null;
            return activeFrom == null
                ? nulllableTimeSpan : (DateTime.Parse(activationDate).Add(TimeSpan.Parse(activeTo)) - DateTime.Parse(activationDate).Add(TimeSpan.Parse(activeFrom)));
        }

        public async Task<bool> PasswordExists(string password)
        => await this.repository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Password == password) == null;
    }
}
