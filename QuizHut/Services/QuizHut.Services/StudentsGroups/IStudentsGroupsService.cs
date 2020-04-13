namespace QuizHut.Services.StudentsGroups
{
    using System.Threading.Tasks;

    public interface IStudentsGroupsService
    {
        Task CreateStudentGroupAsync(string groupId, string studentId);

        Task DeleteAsync(string groupId, string studentId);
    }
}
