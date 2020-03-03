namespace QuizHut.Services.Groups
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Quizzes;
    using QuizHut.Web.ViewModels.Students;

    public interface IGroupsService
    {
        Task<IList<T>> GetAllByCreatorIdAsync<T>(string id);

        Task<string> CreateAsync(string name, string creatorId);

        Task<GroupWithQuizzesViewModel> GetGroupModelAsync(string groupId, string creatorId, IList<QuizAssignViewModel> quizzes);

        Task<GroupDetailsViewModel> GetGroupDetailsModelAsync(string groupId);

        Task AssignQuizzesToGroupAsync(string groupId, List<string> quizzesIds);

        Task AssignStudentsToGroupAsync(string groupId, IList<string> studentsIds);

        Task DeleteAsync(string groupId);

        Task UpdateNameAsync(string groupId, string newName);

        Task DeleteQuizFromGroupAsync(string groupId, string quizId);

        Task DeleteStudentFromGroupAsync(string groupId, string studentId);

        Task<IList<QuizAssignViewModel>> FilterQuizzesThatAreNotAssignedToThisGroup(string qroupId, IList<QuizAssignViewModel> quizzes);

        Task<IList<StudentViewModel>> FilterStudentsThatAreNotAssignedToThisGroup(string qroupId, IList<StudentViewModel> students);
    }
}
