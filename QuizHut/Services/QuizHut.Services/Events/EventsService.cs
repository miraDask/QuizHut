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
    using QuizHut.Services.Messaging;
    using QuizHut.Services.Quizzes;

    public class EventsService : IEventsService
    {
        private readonly IDeletableEntityRepository<Event> repository;
        private readonly IQuizzesService quizService;
        private readonly IEventsGroupsService eventsGroupsService;
        private readonly IEmailSender emailSender;

        public EventsService(
            IDeletableEntityRepository<Event> repository,
            IQuizzesService quizService,
            IEventsGroupsService eventsGroupsService,
            IEmailSender emailSender)
        {
            this.repository = repository;
            this.quizService = quizService;
            this.eventsGroupsService = eventsGroupsService;
            this.emailSender = emailSender;
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
            var query = this.repository.AllAsNoTracking();

            if (groupId != null)
            {
                query = this.repository
                .AllAsNoTrackingWithDeleted()
                .Where(x => !x.EventsGroups.Any(x => x.GroupId == groupId));
            }

            return await query
                .Where(x => x.CreatorId == creatorId)
                .OrderByDescending(x => x.CreatedOn)
                .To<T>()
                .ToListAsync();
        }

        public async Task<IList<T>> GetByStudentIdFilteredByStatus<T>(Status status, string studentId, int page, int countPerPage)
        {
            var query = this.repository.AllAsNoTracking()
                .Where(x => x.EventsGroups.Any(x => x.Group.StudentstGroups.Any(x => x.StudentId == studentId)));

            if (status == Status.Active)
            {
                query = query.Where(x => !x.Results.Any(x => x.StudentId == studentId));
            }

            return await query.Where(x => x.Status == status)
              .OrderByDescending(x => x.CreatedOn)
              .Skip(countPerPage * (page - 1))
              .Take(countPerPage)
              .To<T>()
              .ToListAsync();
        }

        public async Task<IList<T>> GetAllPerPage<T>(int page, int countPerPage)
        => await this.repository.AllAsNoTracking()
              .OrderByDescending(x => x.CreatedOn)
              .Skip(countPerPage * (page - 1))
              .Take(countPerPage)
              .To<T>()
              .ToListAsync();

        // TODO - ???
        public async Task<IList<T>> GetAllFiteredByStatusAsync<T>(
            Status status,
            string creatorId = null,
            string studentId = null,
            string groupId = null)
        {
            var query = this.repository.AllAsNoTracking();

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            if (groupId != null)
            {
                query = query.Where(x => !x.EventsGroups.Any(x => x.GroupId == groupId));
            }

            if (studentId != null)
            {
                query = query.Where(x => x.EventsGroups.Any(x => x.Group.StudentstGroups.Any(x => x.StudentId == studentId)));

                if (status == Status.Active)
                {
                    query = query.Where(x => !x.Results.Any(x => x.StudentId == studentId));
                }
            }

            return await query
              .Where(x => x.Status == status)
              .OrderByDescending(x => x.CreatedOn)
              .To<T>()
              .ToListAsync();
        }

        public async Task<string> CreateEventAsync(string name, string activationDate, string activeFrom, string activeTo, string creatorId)
        {
            var activationDateAndTime = this.GetActivationDateAndTimeLocal(activationDate, activeFrom).ToUniversalTime();
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

            await this.SheduleStatusChange(activationDateAndTime, durationOfActivity, @event.Id);
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
            var now = DateTime.UtcNow;
            var @event = await this.GetEventById(eventId);
            @event.QuizId = quizId;

            if (@event.ActivationDateAndTime <= now
                && @event.ActivationDateAndTime.Add(@event.DurationOfActivity) > now
                && @event.Status != Status.Ended)
            {
                @event.Status = Status.Active;
                var endingDelay = @event.ActivationDateAndTime.Add(@event.DurationOfActivity) - now;
                BackgroundJob.Schedule(() => this.SetStatusChangeJob(eventId, Status.Ended), endingDelay);
            }
            else if (@event.ActivationDateAndTime > now)
            {
                await this.SheduleStatusChange(@event.ActivationDateAndTime, @event.DurationOfActivity, @event.Id);
            }

            this.repository.Update(@event);
            await this.repository.SaveChangesAsync();
            await this.quizService.AssignEventToQuiz(eventId, quizId);
        }

        public async Task DeleteQuizFromEventAsync(string eventId, string quizId)
        {
            var @event = await this.GetEventById(eventId);
            @event.QuizId = null;
            if (@event.Status != Status.Ended)
            {
                @event.Status = Status.Pending;
            }

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
            var activationDateAndTime = this.GetActivationDateAndTimeLocal(activationDate, activeFrom).ToUniversalTime();
            var durationOfActivity = this.GetDurationOfActivity(activationDate, activeFrom, activeTo);
            @event.Name = name;
            @event.ActivationDateAndTime = activationDateAndTime;
            @event.DurationOfActivity = durationOfActivity;
            @event.Status = Status.Pending;
            this.repository.Update(@event);
            await this.repository.SaveChangesAsync();

            if (@event.QuizId == null)
            {
                return;
            }

            await this.SheduleStatusChange(activationDateAndTime, durationOfActivity, id);
        }

        public string GetTimeErrorMessage(string activeFrom, string activeTo, string activationDate)
        {
            var now = DateTime.Now;
            var activationDateAndTime = this.GetActivationDateAndTimeLocal(activationDate, activeFrom);
            if (now.Date > activationDateAndTime.Date)
            {
                return ServicesConstants.InvalidActivationDate;
            }

            var timeToStart = TimeSpan.Parse(activeFrom);
            var timeNow = now.TimeOfDay;
            if (now.Date == activationDateAndTime.Date && timeToStart < timeNow)
            {
                return ServicesConstants.InvalidStartingTime;
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
                await this.eventsGroupsService.CreateEventGroupAsync(eventId, groupId);
            }
        }

        public async Task SheduleStatusChange(DateTime activationDateAndTime, TimeSpan durationOfActivity, string eventId)
        {
            var @event = await this.GetEventById(eventId);
            var now = DateTime.UtcNow;
            var activationDelay = activationDateAndTime - now;
            var endingDelay = activationDateAndTime.Add(durationOfActivity) - now;
            BackgroundJob.Schedule(() => this.SetStatusChangeJob(eventId, Status.Active), activationDelay);
            BackgroundJob.Schedule(() => this.SetStatusChangeJob(eventId, Status.Ended), endingDelay);
        }

        public async Task SetStatusChangeJob(string eventId, Status status)
        {
            var @event = await this.GetEventById(eventId);
            if (@event.QuizId == null)
            {
                return;
            }

            @event.Status = status;
            this.repository.Update(@event);
            await this.repository.SaveChangesAsync();
        }

        public async Task SendEmailsToEventGroups(string eventId, string emailHtmlContent)
        {
            var eventInfo = await this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == eventId)
                .Select(x => new
                {
                    Password = x.Quiz.Password.Content,
                    Emails = x.EventsGroups.SelectMany(x => x.Group.StudentstGroups.Select(x => x.Student.Email)),
                }).FirstOrDefaultAsync();

            emailHtmlContent = emailHtmlContent.Replace(ServicesConstants.StringToReplace, eventInfo.Password);

            foreach (var email in eventInfo.Emails)
            {
                await this.emailSender.SendEmailAsync(
                    ServicesConstants.SenderEmail,
                    ServicesConstants.SenderName,
                    email,
                    ServicesConstants.EventInvitationSubject,
                    emailHtmlContent);
            }
        }

        public async Task<IList<T>> GetAllAsync<T>()
        => await this.repository
                .AllAsNoTracking()
                .To<T>()
                .ToListAsync();

        public int GetEventsCountByStudentIdAndStatus(string id, Status status)
        {
            var query = this.repository.AllAsNoTracking()
                   .Where(x => x.EventsGroups.Any(x => x.Group.StudentstGroups.Any(x => x.StudentId == id)));

            if (status == Status.Active)
            {
                query = query.Where(x => !x.Results.Any(x => x.StudentId == id));
            }

            return query.Where(x => x.Status == status).Count();
        }

        public int GetAllEventsCount() => this.repository.AllAsNoTracking().Count();

        private async Task<Event> GetEventById(string id)
        => await this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

        private DateTime GetActivationDateAndTimeLocal(string activationDate, string activeFrom)
        => DateTime.Parse(activationDate).Add(TimeSpan.Parse(activeFrom));

        private TimeSpan GetDurationOfActivity(string activationDate, string activeFrom, string activeTo)
        => DateTime.Parse(activationDate).Add(TimeSpan.Parse(activeTo)) - DateTime.Parse(activationDate).Add(TimeSpan.Parse(activeFrom));
    }
}
