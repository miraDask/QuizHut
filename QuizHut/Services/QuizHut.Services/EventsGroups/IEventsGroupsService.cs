namespace QuizHut.Services.EventsGroups
{
    using System.Threading.Tasks;

    public interface IEventsGroupsService
    {
        Task CreateEventGroupAsync(string eventId, string groupId);

        Task DeleteAsync(string eventId, string groupId);
    }
}
