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

        public async Task<IEnumerable<T>> GetAllResultsByEventIdAsync<T>(string eventId, string groupName)
        => await this.repository
        .AllAsNoTracking()
        .Where(x => x.EventId == eventId)
        .Where(x => x.Student.StudentsInGroups.Any(x => x.Group.Name == groupName))
        .To<T>()
        .ToListAsync();

        public async Task CreateResultAsync(string studentId, int points, int maxPoints, string quizId)
        {
            var @event = await this.eventRepository
                .AllAsNoTracking()
                .Where(x => x.QuizId == quizId)
                .FirstOrDefaultAsync();
            var result = new Result()
            {
                Points = points,
                StudentId = studentId,
                MaxPoints = maxPoints,
                EventId = @event.Id,
            };

            @event.Results.Add(result);
            await this.repository.AddAsync(result);
            await this.eventRepository.SaveChangesAsync();
            await this.repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllByStudentIdAsync<T>(string id)
        => await this.repository
            .AllAsNoTracking()
            .Where(x => x.StudentId == id)
            .OrderByDescending(x => x.CreatedOn)
            .To<T>()
            .ToListAsync();
    }
}
