namespace QuizHut.Services.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.EventsGroups;
    using QuizHut.Services.Mapping;

    public class EventsService : IEventsService
    {
        private readonly IDeletableEntityRepository<Event> repository;
        private readonly IEventsGroupsService eventGroupsService;

        public EventsService(IDeletableEntityRepository<Event> repository, IEventsGroupsService eventGroupsService)
        {
            this.repository = repository;
            this.eventGroupsService = eventGroupsService;
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
                var assignedEventsIds = await this.eventGroupsService.GetAllEventsIdsByGroupIdAsync(groupId);
                query = query.Where(x => !assignedEventsIds.Contains(x.Id));
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


        public async Task AssignGroupsToEventAsync(string eventId, IList<string> groupsIds)
        {
            foreach (var groupId in groupsIds)
            {
                await this.eventGroupsService.CreateAsync(groupId, eventId);
            }
        }

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

        private async Task<Event> GetEventById(string id)
        => await this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
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
    }
}
