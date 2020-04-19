namespace QuizHut.Services.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Models;
    using QuizHut.Services.Tools.Search;

    public interface IEventsService
    {
        Task<IList<T>> GetAllByCreatorIdAsync<T>(string creatorId);

        Task<IList<T>> GetAllPerPage<T>(int page, int countPerPage, string creatorId = null, string searchCriteria = null, string searchText = null);

        Task<IList<T>> GetAllPerPageByCreatorIdAndStatus<T>(int page, int countPerPage, Status status, string creatorId);

        int GetAllEventsCount(string creatorId = null, string searchCriteria = null, string searchText = null);

        int GetEventsCountByCreatorIdAndStatus(Status status, string creatorId);

        Task<IList<T>> GetAllFiteredByStatusAndGroupAsync<T>(Status status, string groupId, string creatorId = null);

        Task<IList<T>> GetPerPageByStudentIdFilteredByStatusAsync<T>(Status status, string studentId, int page, int countPerPage, bool withDeleted);

        int GetEventsCountByStudentIdAndStatus(string id, Status status);

        Task<IList<T>> GetAllByGroupIdAsync<T>(string groupId);

        Task DeleteAsync(string eventId);

        Task UpdateAsync(string id, string name, string activationDate, string activeFrom, string ativeTo, string timeZone);

        Task AssigQuizToEventAsync(string eventId, string quizId, string timeZone);

        Task AssignGroupsToEventAsync(IList<string> groupIds, string eventId);

        Task<string> CreateEventAsync(string name, string activationDate, string activeFrom, string activeTo, string creatorId);

        Task<T> GetEventModelByIdAsync<T>(string eventId);

        Task DeleteQuizFromEventAsync(string eventId, string quizId);

        string GetTimeErrorMessage(string activeFrom, string activeTo, string activationDate, string timeZone);

        Task SendEmailsToEventGroups(string eventId, string emailHtmlContent);
    }
}
