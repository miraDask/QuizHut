namespace QuizHut.Services.Quizzes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Common;
    using QuizHut.Services.Mapping;
    using QuizHut.Services.Tools.Expressions;

    public class QuizzesService : IQuizzesService
    {
        private readonly IDeletableEntityRepository<Quiz> quizRepository;
        private readonly IExpressionBuilder expressionBuilder;
        private readonly IRepository<Password> passwordRepository;

        public QuizzesService(
            IDeletableEntityRepository<Quiz> quizRepository,
            IExpressionBuilder expressionBuilder,
            IRepository<Password> passwordRepository)
        {
            this.quizRepository = quizRepository;
            this.expressionBuilder = expressionBuilder;
            this.passwordRepository = passwordRepository;
        }

        public async Task<string> CreateQuizAsync(string name, string description, int? timer, string creatorId, string password)
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

        public async Task<IList<T>> GetAllUnAssignedToEventAsync<T>(string creatorId = null)
        {
            var query = this.quizRepository
                   .AllAsNoTracking()
                   .Where(x => x.EventId == null);

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            return await query.OrderByDescending(x => x.CreatedOn)
                .To<T>()
                .ToListAsync();
        }

        public async Task<T> GetQuizByIdAsync<T>(string id)
       => await this.quizRepository
               .AllAsNoTracking()
               .Where(x => x.Id == id)
               .To<T>()
               .FirstOrDefaultAsync();

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

        public async Task<bool[]> HasUserPermition(string userId, string quizId, bool isQuizTaken)
        {
            var quizQuery = this.quizRepository
                .AllAsNoTracking()
                .Where(x => x.Id == quizId);

            var creatorId = await quizQuery.Select(x => x.CreatorId).FirstOrDefaultAsync();
            if (creatorId == userId)
            {
                return new bool[] { true, true };
            }

            if (isQuizTaken)
            {
                return new bool[] { false, false };
            }

            var eventIsActive = await quizQuery.Select(x => x.Event.Status == Status.Active).FirstOrDefaultAsync();
            if (!eventIsActive)
            {
                return new bool[] { false, false };
            }

            var results = await quizQuery
                .SelectMany(x => x.Event.Results.Where(x => x.StudentId == userId))
                .ToListAsync();
            if (results.Count() > 0)
            {
                return new bool[] { false, false };
            }

            var eventGroups = await quizQuery
                .SelectMany(x => x.Event.EventsGroups
                .Where(x => x.Group.StudentstGroups
                .Any(x => x.StudentId == userId)))
                .ToListAsync();

            return eventGroups.Count() > 0 ? new bool[] { true, false } : new bool[] { false, false };
        }

        public async Task AssignQuizToEventAsync(string eventId, string quizId)
        {
            var quiz = await this.quizRepository
                .AllAsNoTracking()
                .Where(x => x.Id == quizId)
                .FirstOrDefaultAsync();

            quiz.EventId = eventId;
            this.quizRepository.Update(quiz);
            await this.quizRepository.SaveChangesAsync();
        }

        public async Task DeleteEventFromQuizAsync(string eventId, string quizId)
        {
            var quiz = await this.quizRepository
                .AllAsNoTracking()
                .Where(x => x.Id == quizId)
                .FirstOrDefaultAsync();

            quiz.EventId = null;
            this.quizRepository.Update(quiz);
            await this.quizRepository.SaveChangesAsync();
        }

        public async Task<IList<T>> GetUnAssignedToCategoryByCreatorIdAsync<T>(string categoryId, string creatorId)
        => await this.quizRepository
            .AllAsNoTracking()
            .Where(x => x.CreatorId == creatorId && x.CategoryId != categoryId)
            .To<T>()
            .ToListAsync();

        public async Task<IList<T>> GetAllByCategoryIdAsync<T>(string id)
        => await this.quizRepository
            .AllAsNoTracking()
            .Where(x => x.CategoryId == id)
            .To<T>()
            .ToListAsync();

        public async Task<string> GetCreatorIdByQuizIdAsync(string id)
        => await this.quizRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.CreatorId)
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<T>> GetAllPerPageAsync<T>(
            int page,
            int countPerPage,
            string creatorId = null,
            string searchCriteria = null,
            string searchText = null,
            string categoryId = null)
        {
            var query = this.quizRepository.AllAsNoTracking();

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            if (categoryId != null)
            {
                query = query.Where(x => x.CategoryId == categoryId);
            }

            var emptyNameInput = searchText == null && searchCriteria == ServicesConstants.Name;
            if (searchCriteria != null && !emptyNameInput)
            {
                var filter = this.expressionBuilder.GetExpression<Quiz>(searchCriteria, searchText);
                query = query.Where(filter);
            }

            return await query.OrderByDescending(x => x.CreatedOn)
            .Skip(countPerPage * (page - 1))
            .Take(countPerPage)
            .To<T>()
            .ToListAsync();
        }

        public async Task<int> GetAllQuizzesCountAsync(string creatorId = null, string searchCriteria = null, string searchText = null, string categoryId = null)
        {
            var query = this.quizRepository.AllAsNoTracking();

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            if (categoryId != null)
            {
                query = query.Where(x => x.CategoryId == categoryId);
            }

            var emptyNameInput = searchText == null && searchCriteria == ServicesConstants.Name;
            if (searchCriteria != null && !emptyNameInput)
            {
                var filter = this.expressionBuilder.GetExpression<Quiz>(searchCriteria, searchText);
                query = query.Where(filter);
            }

            return await query.CountAsync();
        }

        public async Task<string> GetQuizNameByIdAsync(string id)
        => await this.quizRepository.AllAsNoTracking()
             .Where(x => x.Id == id)
             .Select(x => x.Name)
             .FirstOrDefaultAsync();
    }
}
