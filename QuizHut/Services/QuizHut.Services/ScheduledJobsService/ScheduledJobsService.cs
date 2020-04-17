namespace QuizHut.Services.ScheduledJobsService
{
    using System;
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
    using QuizHut.Services.Events;

    public class ScheduledJobsService : IScheduledJobsService
    {
        private readonly IDeletableEntityRepository<ScheduledJob> repository;
        private readonly IDeletableEntityRepository<Event> eventRepository;
        private readonly IHubContext<QuizHub> hub;
        private readonly IBackgroundJobClient backgroundJobClient;

        public ScheduledJobsService(
            IDeletableEntityRepository<ScheduledJob> repository,
            IDeletableEntityRepository<Event> eventRepository,
            IHubContext<QuizHub> hub,
            IBackgroundJobClient backgroundJobClient)
        {
            this.repository = repository;
            this.eventRepository = eventRepository;
            this.hub = hub;
            this.backgroundJobClient = backgroundJobClient;
        }

        public async Task CreateStartEventJobAsync(string eventId, TimeSpan activationDelay)
        {
            var activationScheduleJobId = this.backgroundJobClient.Schedule(() => this.SetStatusChangeJobAsync(eventId, Status.Active), activationDelay);
            var job = new ScheduledJob()
            {
                JobId = activationScheduleJobId,
                EventId = eventId,
                IsActivationJob = true,
            };

            await this.repository.AddAsync(job);
            await this.repository.SaveChangesAsync();
        }

        public async Task CreateEndEventJobAsync(string eventId, TimeSpan endingDelay)
        {
            var endingScheduleJobId = this.backgroundJobClient.Schedule(() => this.SetStatusChangeJobAsync(eventId, Status.Ended), endingDelay);
            var job = new ScheduledJob()
            {
                JobId = endingScheduleJobId,
                EventId = eventId,
                IsActivationJob = false,
            };

            await this.repository.AddAsync(job);
            await this.repository.SaveChangesAsync();
        }

        public async Task DeleteJobsAsync(string eventId, bool all, bool deleteActivationJobCondition = false)
        {
            var query = this.repository
                .AllAsNoTracking()
                .Where(x => x.EventId == eventId);

            if (!all)
            {
                query = query.Where(x => x.IsActivationJob == deleteActivationJobCondition);
            }

            var jobsIds = await query
                .Select(x => x.JobId)
                .ToListAsync();

            foreach (var jobId in jobsIds)
            {
                this.backgroundJobClient.Delete(jobId);
            }
        }

        public async Task SetStatusChangeJobAsync(string eventId, Status status)
        {
            var @event = await this.eventRepository
                .AllAsNoTracking()
                .Where(x => x.Id == eventId)
                .FirstOrDefaultAsync();

            var studentNames = await this.GetStudentsNamesByEventIdAsync(eventId);
            var adminUpdate = status == Status.Active ? "ActiveEventUpdate" : "EndedEventUpdate";
            var studentUpdate = status == Status.Active ? "NewActiveEventMessage" : "NewEndedEventMessage";

            await this.hub.Clients.Group(GlobalConstants.AdministratorRoleName).SendAsync(adminUpdate, @event.Name);

            foreach (var name in studentNames.Distinct())
            {
                await this.hub.Clients.Group(name).SendAsync(studentUpdate);
            }

            if (@event.QuizId == null || @event.Status == status)
            {
                return;
            }

            @event.Status = status;
            this.eventRepository.Update(@event);
            await this.eventRepository.SaveChangesAsync();
            await this.hub.Clients.All.SendAsync("NewEventStatusUpdate", @event.Status.ToString(), @event.Id);
        }

        private async Task<string[]> GetStudentsNamesByEventIdAsync(string id)
        => await this.eventRepository
                   .AllAsNoTracking()
                   .Where(x => x.Id == id)
                   .SelectMany(x => x.EventsGroups.SelectMany(x => x.Group.StudentstGroups.Select(x => x.Student.UserName)))
                   .ToArrayAsync();
    }
}
