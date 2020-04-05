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

    public class ScheduledJobsService : IScheduledJobsService
    {
        private readonly IDeletableEntityRepository<ScheduledJob> repository;
        private readonly IDeletableEntityRepository<Event> eventRepository;
        private readonly IHubContext<QuizHub> hub;

        public ScheduledJobsService(
            IDeletableEntityRepository<ScheduledJob> repository,
            IDeletableEntityRepository<Event> eventRepository,
            IHubContext<QuizHub> hub)
        {
            this.repository = repository;
            this.eventRepository = eventRepository;
            this.hub = hub;
        }

        public async Task CreateStarEventJobAsync(string eventId, TimeSpan activationDelay)
        {
            var activationScheduleJobId = BackgroundJob.Schedule(() => this.SetStatusChangeJobAsync(eventId, Status.Active), activationDelay);
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
            var endingScheduleJobId = BackgroundJob.Schedule(() => this.SetStatusChangeJobAsync(eventId, Status.Ended), endingDelay);
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
                BackgroundJob.Delete(jobId);
            }
        }

        public async Task SetStatusChangeJobAsync(string eventId, Status status)
        {
            var @event = await this.eventRepository
                .AllAsNoTracking()
                .Where(x => x.Id == eventId)
                .FirstOrDefaultAsync();

            if (status == Status.Active)
            {
                await this.hub.Clients.Group(GlobalConstants.AdministratorRoleName).SendAsync("ActiveEventUpdate", @event.Name);
            }
            else
            {
                await this.hub.Clients.Group(GlobalConstants.AdministratorRoleName).SendAsync("EndedEventUpdate", @event.Name);
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
    }
}
