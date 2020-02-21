namespace QuizHut.Services.Group
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Quizzes;

    public interface IGroupService
    {
        Task<IList<T>> GetAllByCreatorIdAsync<T>(string id);

        Task<string> CreateAsync(string name, string creatorId);

        Task<GroupWithQuizzesViewModel> GetGroupModelAsync(string groupId, string creatorId, IList<QuizAssignViewModel> quizzes);

        Task<GroupDetailsViewModel> GetGroupDetailsModelAsync(string groupId);

        Task AssignQuizzesToGroup(string groupId, List<string> quizzesIds);

        Task AssignParticipantsToGroup(string groupId, IList<string> participantsIds);
    }
}
