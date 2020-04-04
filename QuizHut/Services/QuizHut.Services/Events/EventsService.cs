namespace QuizHut.Services.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Hangfire;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using QuizHut.Common;
    using QuizHut.Common.Hubs;
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
        private readonly IHubContext<QuizHub> hub;

        public EventsService(
            IDeletableEntityRepository<Event> repository,
            IQuizzesService quizService,
            IEventsGroupsService eventsGroupsService,
            IEmailSender emailSender,
            IHubContext<QuizHub> hub)
        {
            this.repository = repository;
            this.quizService = quizService;
            this.eventsGroupsService = eventsGroupsService;
            this.emailSender = emailSender;
            this.hub = hub;
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

        public async Task<IList<T>> GetByStudentIdFilteredByStatus<T>(Status status, string studentId, int page, int countPerPage, bool withDeleted)
        {
            var query = withDeleted == true ? this.repository.AllAsNoTrackingWithDeleted() : this.repository.AllAsNoTracking();
            query = query.Where(x => x.EventsGroups.Any(x => x.Group.StudentstGroups.Any(x => x.StudentId == studentId)));

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

        public async Task<IList<T>> GetAllPerPage<T>(int page, int countPerPage, string creatorId = null)
        {
            var query = this.repository.AllAsNoTracking();

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            return await query
                   .OrderByDescending(x => x.CreatedOn)
                   .Skip(countPerPage * (page - 1))
                   .Take(countPerPage)
                   .To<T>()
                   .ToListAsync();
        }

        public async Task<IList<T>> GetAllPerPage<T>(int page, int countPerPage, Status status, string creatorId = null)
        {
            var query = this.repository.AllAsNoTracking().Where(x => x.Status == status);

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            return await query
                   .OrderByDescending(x => x.CreatedOn)
                   .Skip(countPerPage * (page - 1))
                   .Take(countPerPage)
                   .To<T>()
                   .ToListAsync();
        }

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
              .Where(x => x.Status != status)
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
            @event.QuizName = await this.quizService.GetQuizNameByIdAsync(quizId);

            /*&& @event.Status != Status.Ended*/
            if (@event.ActivationDateAndTime <= now
                && @event.ActivationDateAndTime.Add(@event.DurationOfActivity) > now)
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
            @event.Status = this.GetStatus(activationDateAndTime, @event.QuizId);

            this.repository.Update(@event);
            await this.repository.SaveChangesAsync();

            if (@event.QuizId != null)
            {
                await this.SheduleStatusChange(activationDateAndTime, durationOfActivity, id);
            }

            await this.hub.Clients.All.SendAsync("NewEventStatusUpdate", @event.Status.ToString(), @event.Id);
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
            if (now.Date == activationDateAndTime.Date && timeToStart.Minutes < timeNow.Minutes)
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
            if (@event.QuizId == null || @event.Status == status)
            {
                return;
            }

            @event.Status = status;
            this.repository.Update(@event);
            await this.repository.SaveChangesAsync();

            if (status == Status.Active)
            {
                await this.hub.Clients.Group(GlobalConstants.AdministratorRoleName).SendAsync("ActiveEventUpdate", @event.Name);
            }
            else
            {
                await this.hub.Clients.Group(GlobalConstants.AdministratorRoleName).SendAsync("EndedEventUpdate", @event.Name);
            }

            await this.hub.Clients.All.SendAsync("NewEventStatusUpdate", @event.Status.ToString(), @event.Id);
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

        public int GetEventsCountByStudentIdAndStatus(string id, Status status)
        {
            var query = this.repository.AllAsNoTracking()
                   .Where(x => x.EventsGroups.Any(x => x.Group.StudentstGroups.Any(x => x.StudentId == id)))
                   .Where(x => x.Status == status);

            if (status == Status.Active)
            {
                query = query.Where(x => !x.Results.Any(x => x.StudentId == id));
            }

            return query.Count();
        }

        public int GetAllEventsCount(string creatorId = null)
        {
            var query = this.repository.AllAsNoTracking();

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            return query.Count();
        }

        public int GetAllEventsCount(Status status, string creatorId = null)
        {
            var query = this.repository.AllAsNoTracking().Where(x => x.Status == status);

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            return query.Count();
        }

        private async Task<Event> GetEventById(string id)
        => await this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

        private Status GetStatus(DateTime activationDateAndTime, string quizId)
        {
            if (quizId == null)
            {
                return Status.Pending;
            }

            var now = DateTime.UtcNow;
            if (now.Date < activationDateAndTime.Date || activationDateAndTime.TimeOfDay.Minutes > now.TimeOfDay.Minutes)
            {
                return Status.Pending;
            }

            return Status.Active;
        }

        private DateTime GetActivationDateAndTimeLocal(string activationDate, string activeFrom)
        => DateTime.Parse(activationDate).Add(TimeSpan.Parse(activeFrom));

        private TimeSpan GetDurationOfActivity(string activationDate, string activeFrom, string activeTo)
        => DateTime.Parse(activationDate).Add(TimeSpan.Parse(activeTo)) - DateTime.Parse(activationDate).Add(TimeSpan.Parse(activeFrom));
    }
}
