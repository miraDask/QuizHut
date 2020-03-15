namespace QuizHut.Services.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Hangfire;
    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Common;
    using QuizHut.Services.EventsGroups;
    using QuizHut.Services.Mapping;
    using QuizHut.Services.Quizzes;

    public class EventsService : IEventsService
    {
        private readonly IDeletableEntityRepository<Event> repository;
        private readonly IQuizzesService quizService;
        private readonly IEventsGroupsService eventsGroupsService;

        public EventsService(
            IDeletableEntityRepository<Event> repository,
            IQuizzesService quizService,
            IEventsGroupsService eventsGroupsService)
        {
            this.repository = repository;
            this.quizService = quizService;
            this.eventsGroupsService = eventsGroupsService;
        }

        public async Task DeleteAsync(string eventId)
        {
            var @event = await this.GetEventById(eventId);
            var quizId = @event.QuizId;
            if (quizId != null)
            {
                await this.quizService.DeleteEventFromQuiz(eventId, quizId);
            }

            this.repository.Delete(@event);
            await this.repository.SaveChangesAsync();
        }

        public async Task<IList<T>> GetAllByCreatorIdAsync<T>(string creatorId, string groupId = null)
        {
            if (groupId != null)
            {
               return await this.repository
                      .AllAsNoTrackingWithDeleted()
                      .Where(x => !x.EventsGroups.Any(x => x.GroupId == groupId))
                      .Where(x => x.CreatorId == creatorId)
                      .OrderByDescending(x => x.CreatedOn)
                      .To<T>()
                      .ToListAsync();
            }

            return await this.repository
                .AllAsNoTracking()
                .Where(x => x.CreatorId == creatorId)
                .OrderByDescending(x => x.CreatedOn)
                .To<T>()
                .ToListAsync();
        }

        public async Task<string> AddNewEventAsync(string name, string activationDate, string activeFrom, string activeTo, string creatorId)
        {
            var activationDateAndTime = this.GetActivationDateAndTime(activationDate, activeFrom);
            var durationOfActivity = this.GetDurationOfActivity(activationDate, activeFrom, activeTo);
            var @event = new Event
            {
                Name = name,
                Status = Status.Pending,
                ActivationDateAndTime = activationDateAndTime,
                DurationOfActivity = durationOfActivity,
                CreatorId = creatorId,
            };

            await this.repository.AddAsync(@event);
            await this.repository.SaveChangesAsync();

            this.SheduleStatudChange(activationDateAndTime, durationOfActivity, @event.Id);
            return @event.Id;
        }


        public async Task<T> GetEventModelByIdAsync<T>(string eventId)
        => await this.repository
                .AllAsNoTrackingWithDeleted()
                .Where(x => x.Id == eventId)
                .To<T>()
                .FirstOrDefaultAsync();

        public async Task AssigQuizToEventAsync(string eventId, string quizId)
        {
            var @event = await this.GetEventById(eventId);
            @event.QuizId = quizId;
            this.repository.Update(@event);
            await this.repository.SaveChangesAsync();
            await this.quizService.AssignEventToQuiz(eventId, quizId);
        }

        public async Task DeleteQuizFromEventAsync(string eventId, string quizId)
        {
            var @event = await this.GetEventById(eventId);
            @event.QuizId = null;
            this.repository.Update(@event);
            await this.repository.SaveChangesAsync();
            await this.quizService.DeleteEventFromQuiz(eventId, quizId);
        }

        public async Task<IList<T>> GetAllByGroupIdAsync<T>(string groupId)
        => await this.repository
            .AllAsNoTracking()
            .Where(x => x.EventsGroups.Any(x => x.GroupId == groupId))
            .To<T>()
            .ToListAsync();

        public async Task UpdateAsync(string id, string name, string activationDate, string activeFrom, string activeTo)
        {
            var @event = await this.GetEventById(id);
            var activationDateAndTime = this.GetActivationDateAndTime(activationDate, activeFrom);
            var durationOfActivity = this.GetDurationOfActivity(activationDate, activeFrom, activeTo);
            @event.Name = name;
            @event.ActivationDateAndTime = activationDateAndTime;
            @event.DurationOfActivity = durationOfActivity;
            this.repository.Update(@event);
            await this.repository.SaveChangesAsync();
            this.SheduleStatudChange(activationDateAndTime, durationOfActivity, id);
        }

        public string GetTimeErrorMessage(string activeFrom, string activeTo, string activationDate)
        {
            var now = DateTime.UtcNow;
            var activationDateAndTime = this.GetActivationDateAndTime(activationDate, activeFrom);
            if (now.Date > activationDateAndTime.Date)
            {
                return ServicesConstants.InvalidActivationDate;
            }

            var duration = this.GetDurationOfActivity(activationDate, activeFrom, activeTo);
            if (duration.Hours <= 0 && duration.Minutes <= 0)
            {
                return ServicesConstants.InvalidDurationOfActivity;
            }

            return null;
        }

        public async Task AssignGroupsToEventAsync(IList<string> groupIds, string eventId)
        {
            foreach (var groupId in groupIds)
            {
                await this.eventsGroupsService.CreateAsync(eventId, groupId);
            }
        }

        public void SheduleStatudChange(DateTime activationDateAndTime, TimeSpan durationOfActivity, string eventId)
        {
            var now = DateTime.UtcNow;
            var activationDelay = activationDateAndTime - now;
            var endingDelay = activationDateAndTime.Add(durationOfActivity) - now;
            BackgroundJob.Schedule(() => this.SetStatusChangeJob(eventId, Status.Active), activationDelay);
            BackgroundJob.Schedule(() => this.SetStatusChangeJob(eventId, Status.Ended), endingDelay);
        }

        public async Task SetStatusChangeJob(string eventId, Status status)
        {
            var @event = await this.GetEventById(eventId);
            @event.Status = status;
            this.repository.Update(@event);
            await this.repository.SaveChangesAsync();
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
