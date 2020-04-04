namespace QuizHut.Services.ScheduledJobsService
{
    using System;
    using System.Threading.Tasks;

    public interface IScheduledJobsService
    {
        Task DeleteJobsAsync(string eventId, bool all, bool deleteActivationJobCondition = false);

        Task CreateStarEventJobAsync(string eventId, TimeSpan activationDelay);

        Task CreateEndEventJobAsync(string eventId, TimeSpan endingDelay);
    }
}
