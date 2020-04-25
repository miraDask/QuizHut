namespace QuizHut.Services.Users
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUsersService
    {
        Task<IList<T>> GetAllStudentsAsync<T>(string teacherId = null, string groupId = null);

        Task<IList<T>> GetAllInRolesPerPageAsync<T>(
            int page,
            int countPerPage,
            string searchCriteria = null,
            string searchText = null,
            string roleId = null);

        Task<int> GetAllInRolesCountAsync(
            string searchCriteria = null,
            string searchText = null,
            string roleId = null);

        Task<IList<T>> GetAllByGroupIdAsync<T>(string groupId);

        Task<bool> AddStudentAsync(string email, string teacherId);

        Task DeleteFromTeacherListAsync(string studentId, string teacherId);

        Task<int> GetAllStudentsCountAsync(string teacher = null, string searchCriteria = null, string searchText = null);

        Task<IEnumerable<T>> GetAllStudentsPerPageAsync<T>(int page, int countPerPage, string teacherId = null, string searchCriteria = null, string searchText = null);
    }
}
