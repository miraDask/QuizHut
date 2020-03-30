namespace QuizHut.Services.Events
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using QuizHut.Data.Common.Enumerations;

    public interface IEventsService
    {
        Task<IList<T>> GetAllByCreatorIdAsync<T>(string creatorId, string groupId = null);

        Task<IList<T>> GetAllAsync<T>();

        Task<IList<T>> GetAllFiteredByStatusAsync<T>(Status status, string creatorId = null, string studentId = null, string groupId = null);

        Task<IList<T>> GetByStudentIdFilteredByStatus<T>(Status status, string studentId, int page, int countPerPage);

        int GetEventsCountByStudentIdAndStatus(string id, Status status);

        Task<IList<T>> GetAllByGroupIdAsync<T>(string groupId);

        Task DeleteAsync(string eventId);

        Task UpdateAsync(string id, string name, string activationDate, string activeFrom, string ativeTo);

        Task AssigQuizToEventAsync(string eventId, string quizId);

        Task AssignGroupsToEventAsync(IList<string> groupIds, string eventId);

        Task<string> CreateEventAsync(string name, string activationDate, string activeFrom, string activeTo, string creatorId);

        Task<T> GetEventModelByIdAsync<T>(string eventId);

        Task DeleteQuizFromEventAsync(string eventId, string quizId);

        string GetTimeErrorMessage(string activeFrom, string activeTo, string activationDate);

        Task SendEmailsToEventGroups(string eventId, string emailHtmlContent);
    }
}
