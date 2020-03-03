namespace QuizHut.Services.Users
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUsersService
    {
        Task<IList<T>> GetAllByUserIdAsync<T>(string id);

        Task<IList<T>> GetAllByRoleAsync<T>(string roleName);

        Task<bool> AddAsync(string email, string teacherId);

        Task DeleteAsync(string id, string teacherId);

        Task<bool> AssignRoleAsync(string email, string roleName);

        Task RemoveFromRoleAsync(string id, string roleName);
    }
}
