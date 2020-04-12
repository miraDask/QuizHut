namespace QuizHut.Services.Groups
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGroupsService
    {
        Task<IList<T>> GetAllByCreatorIdAsync<T>(string id, string eventId = null);

        Task<IList<T>> GetAllPerPage<T>(int page, int countPerPage, string creatorId = null);

        Task<string> CreateGroupAsync(string name, string creatorId);

        Task<T> GetGroupModelAsync<T>(string groupId);

        int GetAllGroupsCount(string creatorId = null);

        Task<IEnumerable<T>> GetAllByEventIdAsync<T>(string eventId);

        Task AssignEventsToGroupAsync(string groupId, IList<string> eventsIds);

        Task AssignStudentsToGroupAsync(string groupId, IList<string> studentsIds);

        Task DeleteAsync(string groupId);

        Task UpdateNameAsync(string groupId, string newName);

        Task DeleteEventFromGroupAsync(string groupId, string eventId);

        Task DeleteStudentFromGroupAsync(string groupId, string studentId);
    }
}
