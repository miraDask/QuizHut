namespace QuizHut.Services.Results
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class ResultsService : IResultsService
    {
        private readonly IDeletableEntityRepository<Result> repository;
        private readonly IDeletableEntityRepository<Event> eventRepository;

        public ResultsService(
            IDeletableEntityRepository<Result> repository,
            IDeletableEntityRepository<Event> eventRepository)
        {
            this.repository = repository;
            this.eventRepository = eventRepository;
        }

        public async Task<IEnumerable<T>> GetAllResultsByEventAndGroupPerPageAsync<T>(string eventId, string groupId, int page, int countPerPage)
        => await this.repository
                     .AllAsNoTracking()
                     .Where(x => x.EventId == eventId)
                     .Where(x => x.Student.StudentsInGroups.Any(x => x.Group.Id == groupId))
                     .OrderBy(x => x.CreatedOn)
                     .Skip(countPerPage * (page - 1))
                     .Take(countPerPage)
                     .To<T>()
                     .ToListAsync();

        public int GetAllResultsByEventAndGroupCount(string eventId, string groupId)
        => this.repository
               .AllAsNoTracking()
               .Where(x => x.EventId == eventId)
               .Where(x => x.Student.StudentsInGroups.Any(x => x.Group.Id == groupId))
               .Count();

        public async Task<string> CreateResultAsync(string studentId, int points, int maxPoints, string quizId)
        {
            var eventWithQuiz = await this.eventRepository
                .All()
                .Where(x => x.QuizId == quizId)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    QuizName = x.Quiz.Name,
                    x.ActivationDateAndTime,
                })
                .FirstOrDefaultAsync();

            var result = new Result()
            {
                Points = points,
                StudentId = studentId,
                MaxPoints = maxPoints,
                EventId = eventWithQuiz.Id,
                EventName = eventWithQuiz.Name,
                QuizName = eventWithQuiz.QuizName,
                EventActivationDateAndTime = eventWithQuiz.ActivationDateAndTime,
            };

            await this.repository.AddAsync(result);
            await this.repository.SaveChangesAsync();

            var @event = await this.eventRepository
                .All()
                .Where(x => x.QuizId == quizId)
                .FirstOrDefaultAsync();
            @event.Results.Add(result);
            this.eventRepository.Update(@event);
            await this.eventRepository.SaveChangesAsync();

            return result.Id;
        }

        public async Task<IEnumerable<T>> GetPerPageByStudentIdAsync<T>(string id, int page, int countPerPage)
       => await this.repository
           .AllAsNoTracking()
           .Where(x => x.StudentId == id)
           .OrderByDescending(x => x.CreatedOn)
           .Skip(countPerPage * (page - 1))
           .Take(countPerPage)
           .To<T>()
           .ToListAsync();

        public async Task<IEnumerable<T>> GetAllByStudentIdAsync<T>(string id)
        => await this.repository
            .AllAsNoTracking()
            .Where(x => x.StudentId == id)
            .OrderByDescending(x => x.CreatedOn)
            .To<T>()
            .ToListAsync();

        public int GetResultsCountByStudentId(string id)
         => this.repository
            .AllAsNoTracking()
            .Where(x => x.StudentId == id)
            .Count();

        public int GetResultsCountByGroupId(string id)
         => this.repository
            .AllAsNoTracking()
            .Where(x => x.EventId == id)
            .Count();

        public async Task<string> GetQuizNameByEventIdAndStudentIdAsync(string eventId, string studentId)
        => await this.repository.AllAsNoTracking()
                 .Where(x => x.EventId == eventId && x.StudentId == studentId)
                 .Select(x => x.QuizName)
                 .FirstOrDefaultAsync();
    }
}
