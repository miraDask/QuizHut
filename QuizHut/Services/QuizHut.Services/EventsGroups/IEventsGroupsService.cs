namespace QuizHut.Services.EventsGroups
{
    using System.Threading.Tasks;

    public interface IEventsGroupsService
    {
        Task CreateAsync(string groupId, string eventId);

        Task DeleteAsync(string groupId, string eventId);

        Task<string[]> GetAllEventsIdsByGroupIdAsync(string groupId);
    }
}
