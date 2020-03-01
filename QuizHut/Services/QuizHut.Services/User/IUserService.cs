namespace QuizHut.Services.User
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserService
    {
        Task<IList<T>> GetAllByUserIdAsync<T>(string id);

        Task<IList<T>> GetAllByRoleAsync<T>(string roleName);

        Task<bool> AddAsync(string email, string managerId);

        Task DeleteAsync(string id, string managerId);

        Task<bool> AssignRoleAsync(string email, string roleName);

        Task RemoveFromRoleAsync(string id, string roleName);
    }
}
