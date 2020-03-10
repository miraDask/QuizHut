namespace QuizHut.Services.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class EventsService : IEventsService
    {
        private readonly IDeletableEntityRepository<Event> repository;

        public EventsService(IDeletableEntityRepository<Event> repository)
        {
            this.repository = repository;
        }

        public async Task DeleteAsync(string eventId)
        {
            var @event = await this.GetEventById(eventId);
            this.repository.Delete(@event);
            await this.repository.SaveChangesAsync();
        }

        public async Task<IList<T>> GetAllByCreatorIdAsync<T>(string creatorId, string groupId = null)
        {
            var query = this.repository.AllAsNoTracking();

            if (groupId != null)
            {
                query = query.Where(x => x.GroupId != groupId);
            }

            return await query.Where(x => x.CreatorId == creatorId)
                              .OrderByDescending(x => x.CreatedOn)
                              .To<T>()
                              .ToListAsync();
        }

        public async Task<string> AddNewEventAsync(string name, string activationDate, string activeFrom, string activeTo, string creatorId)
        {
            var @event = new Event
            {
                Name = name,
                ActivationDateAndTime = this.GetActivationDateAndTime(activationDate, activeFrom),
                DurationOfActivity = this.GetDurationOfActivity(activationDate, activeFrom, activeTo),
                CreatorId = creatorId,
            };

            await this.repository.AddAsync(@event);
            await this.repository.SaveChangesAsync();

            return @event.Id;
        }

        public async Task<T> GetEventModelByIdAsync<T>(string eventId)
        => await this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == eventId)
                .To<T>()
                .FirstOrDefaultAsync();

        public async Task AssigQuizToEventAsync(string eventId, string quizId)
        {
            var @event = await this.GetEventById(eventId);

            @event.QuizId = quizId;
            this.repository.Update(@event);
            await this.repository.SaveChangesAsync();
        }

        public async Task DeleteQuizFromEventAsync(string eventId, string quizId)
        {
            var @event = await this.GetEventById(eventId);
            @event.QuizId = null;
            this.repository.Update(@event);
            await this.repository.SaveChangesAsync();
        }

        public async Task<IList<T>> GetAllByGroupIdAsync<T>(string groupId)
        => await this.repository
            .AllAsNoTracking()
            .Where(x => x.GroupId == groupId)
            .To<T>()
            .ToListAsync();

        public async Task<IEnumerable<T>> GetAllresultsByEventIdAsync<T>(string eventId)
        {
            var eventQuery = this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == eventId);
            var studentsIds = await eventQuery
                .SelectMany(x => x.Group.StudentstGroups.Select(x => x.StudentId))
                .ToListAsync();

            var quizResultQuery = eventQuery
                .SelectMany(x => x.Quiz.QuizzesResults
                .Where(x => studentsIds.Contains(x.Result.StudentId)));

            return await quizResultQuery.Select(x => x.Result).To<T>().ToListAsync();
        }

        private async Task<Event> GetEventById(string id)
        => await this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

        private DateTime GetActivationDateAndTime(string activationDate, string activeFrom)
        => DateTime.Parse(activationDate).Add(TimeSpan.Parse(activeFrom));

        private TimeSpan GetDurationOfActivity(string activationDate, string activeFrom, string activeTo)
        => DateTime.Parse(activationDate).Add(TimeSpan.Parse(activeTo)) - DateTime.Parse(activationDate).Add(TimeSpan.Parse(activeFrom));
    }
}
