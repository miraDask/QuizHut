namespace QuizHut.Services.Users
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUsersService
    {
        Task<IList<T>> GetAllByUserIdAsync<T>(string id = null, string groupId = null);

        Task<IList<T>> GetAllByRoleAsync<T>(string roleName);

        Task<IList<T>> GetAllByGroupIdAsync<T>(string groupId);

        Task<bool> AddStudentAsync(string email, string teacherId);

        Task DeleteFromTeacherListAsync(string studentId, string teacherId);

        int GetAllStudentsCount(string teacher = null);

        Task<IEnumerable<T>> GetAllPerPage<T>(int page, int countPerPage, string teacherId = null);
    }
}
