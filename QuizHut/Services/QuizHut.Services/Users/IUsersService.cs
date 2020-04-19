namespace QuizHut.Services.Users
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUsersService
    {
        Task<IList<T>> GetAllStudentsAsync<T>(string teacherId = null, string groupId = null);

        Task<IList<T>> GetAllByRoleAsync<T>(string roleName);

        Task<IList<T>> GetAllByGroupIdAsync<T>(string groupId);

        Task<bool> AddStudentAsync(string email, string teacherId);

        Task DeleteFromTeacherListAsync(string studentId, string teacherId);

        int GetAllStudentsCount(string teacher = null, string searchCriteria = null, string searchText = null);

        Task<IEnumerable<T>> GetAllStudentsPerPageAsync<T>(int page, int countPerPage, string teacherId = null, string searchCriteria = null, string searchText = null);
    }
}
