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
        private readonly IDeletableEntityRepository<Quiz> quizRepository;
        private readonly IRepository<Password> passwordRepository;

        public QuizzesService(
            IDeletableEntityRepository<Quiz> quizRepository,
            IRepository<Password> passwordRepository)
        {
            this.quizRepository = quizRepository;
            this.passwordRepository = passwordRepository;
        }

        public async Task<string> AddNewQuizAsync(string name, string description, int? timer, string creatorId, string password)
        {
            var quiz = new Quiz
            {
                Name = name,
                Description = description,
                Timer = timer,
                CreatorId = creatorId,
            };

            var passwordEntitiy = new Password() { Content = password, QuizId = quiz.Id };
            await this.passwordRepository.AddAsync(passwordEntitiy);
            await this.passwordRepository.SaveChangesAsync();

            quiz.PasswordId = passwordEntitiy.Id;
            await this.quizRepository.AddAsync(quiz);
            await this.quizRepository.SaveChangesAsync();

            return quiz.Id;
        }

        public async Task<IList<T>> GetAllByCreatorIdAsync<T>(string id)
          => await this.quizRepository
                 .AllAsNoTracking()
                 .Where(x => x.CreatorId == id)
                 .OrderByDescending(x => x.CreatedOn)
                 .To<T>()
                 .ToListAsync();

        public async Task<T> GetQuizByIdAsync<T>(string id)
       => await this.quizRepository
               .AllAsNoTracking()
               .Where(x => x.Id == id)
               .To<T>()
               .FirstOrDefaultAsync();

        public async Task<IList<T>> GetAllAsync<T>()
         => await this.quizRepository
                .AllAsNoTracking()
                .To<T>()
                .ToListAsync();

        public async Task DeleteByIdAsync(string id)
        {
            var quiz = await this.quizRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            var password = await this.passwordRepository
                .AllAsNoTracking()
                .Where(x => x.QuizId == id)
                .FirstOrDefaultAsync();
            this.passwordRepository.Delete(password);
            this.quizRepository.Delete(quiz);

            await this.passwordRepository.SaveChangesAsync();
            await this.quizRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(string id, string name, string description, int? timer, string password)
        {
            var quiz = await this.quizRepository
               .AllAsNoTracking()
               .FirstOrDefaultAsync(x => x.Id == id);
            var passwordEntity = await this.passwordRepository
                .AllAsNoTracking()
                .Where(x => x.QuizId == id)
                .FirstOrDefaultAsync();

            if (passwordEntity.Content != password)
            {
                passwordEntity.Content = password;
                this.passwordRepository.Update(passwordEntity);
                await this.passwordRepository.SaveChangesAsync();
            }

            quiz.Name = name;
            quiz.Description = description;
            quiz.Timer = timer;

            this.quizRepository.Update(quiz);
            await this.quizRepository.SaveChangesAsync();
        }

        public async Task<string> GetQuizIdByPasswordAsync(string password)
        => await this.quizRepository.AllAsNoTracking()
            .Where(x => x.Password.Content == password)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();
    }
}
