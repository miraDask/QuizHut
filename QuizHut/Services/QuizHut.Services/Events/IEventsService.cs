namespace QuizHut.Services.Events
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEventsService
    {
        Task<IList<T>> GetAllByCreatorIdAsync<T>(string creatorId, string groupId = null);

        Task DeleteAsync(string eventId);

        Task AssignGroupsToEventAsync(string eventId, IList<string> groupsIds);

        Task AssigQuizToEventAsync(string eventId, string quizId);

        Task<string> AddNewEventAsync(string name, string activationDate, string activeFrom, string activeTo, string creatorId);

        Task<T> GetEventModelByIdAsync<T>(string eventId);

        Task DeleteQuizFromEventAsync(string eventId, string quizId);
    }
}
