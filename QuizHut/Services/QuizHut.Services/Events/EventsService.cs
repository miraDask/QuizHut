namespace QuizHut.Services.Events
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

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
    using QuizHut.Services.ScheduledJobsService;
    using QuizHut.Services.Tools.Expressions;
    using TimeZoneConverter;

    public class EventsService : IEventsService
    {
        private readonly IDeletableEntityRepository<Event> repository;
        private readonly IQuizzesService quizService;
        private readonly IEventsGroupsService eventsGroupsService;
        private readonly IScheduledJobsService scheduledJobsService;
        private readonly IEmailSender emailSender;
        private readonly IExpressionBuilder expressionBuilder;
        private readonly IHubContext<QuizHub> hub;

        public EventsService(
            IDeletableEntityRepository<Event> repository,
            IQuizzesService quizService,
            IEventsGroupsService eventsGroupsService,
            IScheduledJobsService scheduledJobsService,
            IEmailSender emailSender,
            IExpressionBuilder expressionBuilder,
            IHubContext<QuizHub> hub)
        {
            this.repository = repository;
            this.quizService = quizService;
            this.eventsGroupsService = eventsGroupsService;
            this.scheduledJobsService = scheduledJobsService;
            this.emailSender = emailSender;
            this.expressionBuilder = expressionBuilder;
            this.hub = hub;
        }

        public async Task DeleteAsync(string eventId)
        {
            var @event = await this.GetEventById(eventId);
            var quizId = @event.QuizId;
            if (quizId != null)
            {
                await this.quizService.DeleteEventFromQuizAsync(eventId, quizId);
            }

            this.repository.Delete(@event);
            await this.repository.SaveChangesAsync();
        }

        public async Task<IList<T>> GetAllByCreatorIdAsync<T>(string creatorId)
        => await this.repository.AllAsNoTracking()
                 .Where(x => x.CreatorId == creatorId)
                 .OrderByDescending(x => x.CreatedOn)
                 .To<T>()
                 .ToListAsync();

        public async Task<IList<T>> GetPerPageByStudentIdFilteredByStatusAsync<T>(
            Status status,
            string studentId,
            int page,
            int countPerPage,
            bool withDeleted,
            string searchCriteria = null,
            string searchText = null)
        {
            var query = withDeleted == true ? this.repository.AllAsNoTrackingWithDeleted() : this.repository.AllAsNoTracking();
            query = query.Where(x => x.EventsGroups.Any(x => x.Group.StudentstGroups.Any(x => x.StudentId == studentId)));

            if (status == Status.Active)
            {
                query = query.Where(x => !x.Results.Any(x => x.StudentId == studentId));
            }

            if (searchCriteria != null && searchText != null)
            {
                var filter = this.expressionBuilder.GetExpression<Event>(searchCriteria, searchText);
                query = query.Where(filter);
            }

            return await query.Where(x => x.Status == status)
              .OrderByDescending(x => x.CreatedOn)
              .Skip(countPerPage * (page - 1))
              .Take(countPerPage)
              .To<T>()
              .ToListAsync();
        }

        public async Task<IList<T>> GetAllPerPage<T>(
            int page,
            int countPerPage,
            string creatorId = null,
            string searchCriteria = null,
            string searchText = null)
        {
            var query = this.repository.AllAsNoTracking();

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            var emptyNameInput = searchText == null && searchCriteria == ServicesConstants.Name;
            if (searchCriteria != null && !emptyNameInput)
            {
                var filter = this.expressionBuilder.GetExpression<Event>(searchCriteria, searchText);
                query = query.Where(filter);
            }

            return await query
                   .OrderByDescending(x => x.CreatedOn)
                   .Skip(countPerPage * (page - 1))
                   .Take(countPerPage)
                   .To<T>()
                   .ToListAsync();
        }

        public async Task<IList<T>> GetAllPerPageByCreatorIdAndStatus<T>(
            int page,
            int countPerPage,
            Status status,
            string creatorId,
            string searchCriteria = null,
            string searchText = null)
        {
            var query = this.repository.AllAsNoTracking().Where(x => x.Status == status && x.CreatorId == creatorId);

            if (searchCriteria != null && searchText != null)
            {
                var filter = this.expressionBuilder.GetExpression<Event>(searchCriteria, searchText);
                query = query.Where(filter);
            }

            return await query
                   .OrderByDescending(x => x.CreatedOn)
                   .Skip(countPerPage * (page - 1))
                   .Take(countPerPage)
                   .To<T>()
                   .ToListAsync();
        }

        public async Task<IList<T>> GetAllFiteredByStatusAndGroupAsync<T>(
            Status status, string groupId, string creatorId = null)
        {
            var query = this.repository.AllAsNoTracking().Where(x => !x.EventsGroups.Any(x => x.GroupId == groupId));

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            return await query
              .Where(x => x.Status != status)
              .OrderByDescending(x => x.CreatedOn)
              .To<T>()
              .ToListAsync();
        }

        public async Task<string> CreateEventAsync(string name, string activationDate, string activeFrom, string activeTo, string creatorId)
        {
            var activationDateAndTime = this.GetActivationDateAndTimeUtc(activationDate, activeFrom);
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

        public async Task AssigQuizToEventAsync(string eventId, string quizId, string timeZone)
        {
            var @event = await this.GetEventById(eventId);
            @event.QuizId = quizId;
            @event.QuizName = await this.quizService.GetQuizNameByIdAsync(quizId);
            @event.Status = this.GetStatus(@event.ActivationDateAndTime, @event.DurationOfActivity, quizId, timeZone);
            this.repository.Update(@event);
            await this.repository.SaveChangesAsync();

            if (@event.Status != Status.Ended)
            {
                await this.SheduleStatusChangeAsync(@event.ActivationDateAndTime, @event.DurationOfActivity, @event.Id, @event.Name, @event.Status, timeZone);
            }

            await this.quizService.AssignQuizToEventAsync(eventId, quizId);
        }

        public async Task DeleteQuizFromEventAsync(string eventId, string quizId)
        {
            var @event = await this.GetEventById(eventId);
            @event.QuizId = null;

            if (@event.Status == Status.Active)
            {
                @event.Status = Status.Pending;
                await this.scheduledJobsService.DeleteJobsAsync(@event.Id, true);
            }

            this.repository.Update(@event);
            await this.repository.SaveChangesAsync();
            await this.quizService.DeleteEventFromQuizAsync(eventId, quizId);
        }

        public async Task<IList<T>> GetAllByGroupIdAsync<T>(string groupId)
        => await this.repository
            .AllAsNoTracking()
            .Where(x => x.EventsGroups.Any(x => x.GroupId == groupId))
            .To<T>()
            .ToListAsync();

        public async Task UpdateAsync(string id, string name, string activationDate, string activeFrom, string activeTo, string timeZone)
        {
            var @event = await this.GetEventById(id);
            var activationDateAndTime = this.GetActivationDateAndTimeUtc(activationDate, activeFrom);
            var durationOfActivity = this.GetDurationOfActivity(activationDate, activeFrom, activeTo);

            @event.Name = name;
            @event.ActivationDateAndTime = activationDateAndTime;
            @event.DurationOfActivity = durationOfActivity;
            @event.Status = this.GetStatus(activationDateAndTime, durationOfActivity, @event.QuizId, timeZone);

            this.repository.Update(@event);
            await this.repository.SaveChangesAsync();

            if (@event.QuizId != null)
            {
                await this.SheduleStatusChangeAsync(activationDateAndTime, durationOfActivity, id, @event.Name, @event.Status, timeZone);
            }

            await this.hub.Clients.All.SendAsync("NewEventStatusUpdate", @event.Status.ToString(), @event.Id);
        }

        public string GetTimeErrorMessage(string activeFrom, string activeTo, string activationDate, string timeZone)
        {
            var zone = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timeZone));
            var userLocalTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
            var activationDateAndTimeUtc = this.GetActivationDateAndTimeUtc(activationDate, activeFrom);
            var activationDateAndTimeToUserLocalTime = TimeZoneInfo.ConvertTimeFromUtc(activationDateAndTimeUtc, zone);

            if (userLocalTimeNow.Date > activationDateAndTimeToUserLocalTime.Date)
            {
                return ServicesConstants.InvalidActivationDate;
            }

            var timeToStart = TimeSpan.Parse(activeFrom);
            var timeNow = userLocalTimeNow.TimeOfDay;
            var startHours = timeToStart.Hours;
            var nowHours = timeNow.Hours;
            var startMins = timeToStart.Minutes;
            var nowMins = timeNow.Minutes;
            var invalidStartingTime = startHours < nowHours || (startHours == nowHours && startMins < nowMins);

            if (userLocalTimeNow.Date == activationDateAndTimeToUserLocalTime.Date && invalidStartingTime)
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

            emailHtmlContent = emailHtmlContent.Replace(GlobalConstants.EmailSender.StringToReplace, eventInfo.Password);

            foreach (var email in eventInfo.Emails)
            {
                await this.emailSender.SendEmailAsync(
                    GlobalConstants.EmailSender.SenderEmail,
                    GlobalConstants.EmailSender.SenderName,
                    email,
                    GlobalConstants.EmailSender.EventInvitationSubject,
                    emailHtmlContent);
            }
        }

        public async Task<int> GetEventsCountByStudentIdAndStatusAsync(string id, Status status, string searchCriteria = null, string searchText = null)
        {
            var query = this.repository
                            .AllAsNoTracking()
                            .Where(x => x.EventsGroups.Any(x => x.Group.StudentstGroups.Any(x => x.StudentId == id)))
                            .Where(x => x.Status == status);

            if (status == Status.Active)
            {
                query = query.Where(x => !x.Results.Any(x => x.StudentId == id));
            }

            if (searchCriteria != null && searchText != null)
            {
                var filter = this.expressionBuilder.GetExpression<Event>(searchCriteria, searchText);
                query = query.Where(filter);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetAllEventsCountAsync(string creatorId = null, string searchCriteria = null, string searchText = null)
        {
            var query = this.repository.AllAsNoTracking();

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            var emptyNameInput = searchText == null && searchCriteria == ServicesConstants.Name;
            if (searchCriteria != null && !emptyNameInput)
            {
                var filter = this.expressionBuilder.GetExpression<Event>(searchCriteria, searchText);
                query = query.Where(filter);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetEventsCountByCreatorIdAndStatusAsync(
            Status status,
            string creatorId,
            string searchCriteria = null,
            string searchText = null)
        {
            var query = this.repository.AllAsNoTracking().Where(x => x.Status == status && x.CreatorId == creatorId);

            if (searchCriteria != null && searchText != null)
            {
                var filter = this.expressionBuilder.GetExpression<Event>(searchCriteria, searchText);
                query = query.Where(filter);
            }

            return await query.CountAsync();
        }

        private async Task<string[]> GetStudentsNamesByEventIdAsync(string id)
        => await this.repository
                     .AllAsNoTracking()
                     .Where(x => x.Id == id)
                     .SelectMany(x => x.EventsGroups.SelectMany(x => x.Group.StudentstGroups.Select(x => x.Student.UserName)))
                     .ToArrayAsync();

        private async Task SheduleStatusChangeAsync(
            DateTime activationDateAndTime,
            TimeSpan durationOfActivity,
            string eventId,
            string eventName,
            Status eventStatus,
            string timeZone)
        {
            var zone = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timeZone));
            var userLocalTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
            var userTimeToUtc = userLocalTimeNow.ToUniversalTime();

            var activationDelay = activationDateAndTime - userTimeToUtc;
            var endingDelay = activationDateAndTime.Add(durationOfActivity) - userTimeToUtc;
            var studentsNames = await this.GetStudentsNamesByEventIdAsync(eventId);

            if (eventStatus == Status.Active)
            {
                foreach (var name in studentsNames.Distinct())
                {
                    await this.hub.Clients.Group(name).SendAsync("NewActiveEventMessage");
                }

                await this.hub.Clients.Group(GlobalConstants.AdministratorRoleName).SendAsync("ActiveEventUpdate", eventName);
                await this.scheduledJobsService.DeleteJobsAsync(eventId, false);
                await this.scheduledJobsService.CreateEndEventJobAsync(eventId, endingDelay);
            }
            else
            {
                foreach (var name in studentsNames.Distinct())
                {
                    await this.hub.Clients.Group(name).SendAsync("NewPendingEventMessage");
                }

                await this.scheduledJobsService.DeleteJobsAsync(eventId, true);
                await this.scheduledJobsService.CreateStartEventJobAsync(eventId, activationDelay);
                await this.scheduledJobsService.CreateEndEventJobAsync(eventId, endingDelay);
            }
        }

        private async Task<Event> GetEventById(string id)
        => await this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

        private Status GetStatus(DateTime activationDateAndTime, TimeSpan durationOfActivity, string quizId, string timeZone)
        {
            var zone = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timeZone));
            var userLocalTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
            var activationDateAndTimeToUserLocalTime = TimeZoneInfo.ConvertTimeFromUtc(activationDateAndTime, zone);

            if (quizId == null
                || userLocalTimeNow.Date < activationDateAndTimeToUserLocalTime.Date
                || activationDateAndTimeToUserLocalTime.TimeOfDay > userLocalTimeNow.TimeOfDay)
            {
                return Status.Pending;
            }

            var startHours = activationDateAndTimeToUserLocalTime.TimeOfDay.Hours;
            var nowHours = userLocalTimeNow.TimeOfDay.Hours;
            var startMins = activationDateAndTimeToUserLocalTime.TimeOfDay.Minutes;
            var nowMins = userLocalTimeNow.TimeOfDay.Minutes;

            var endHours = activationDateAndTimeToUserLocalTime.Add(durationOfActivity).TimeOfDay.Hours;
            var endMinutes = activationDateAndTimeToUserLocalTime.Add(durationOfActivity).TimeOfDay.Minutes;

            if (startHours <= nowHours && startMins <= nowMins
                && (endHours > nowHours || (endHours == nowHours && endMinutes >= nowMins)))
            {
                return Status.Active;
            }

            return Status.Ended;
        }

        private DateTime GetActivationDateAndTimeUtc(string activationDate, string activeFrom)
        => DateTime.ParseExact(activationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).Add(TimeSpan.Parse(activeFrom)).ToUniversalTime();

        private TimeSpan GetDurationOfActivity(string activationDate, string activeFrom, string activeTo)
        => DateTime.ParseExact(activationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).Add(TimeSpan.Parse(activeTo)).ToUniversalTime()
            - this.GetActivationDateAndTimeUtc(activationDate, activeFrom);
    }
}
