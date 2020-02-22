namespace QuizHut.Services.User
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserService
    {
        Task<IList<T>> GetAllByUserIdAsync<T>(string id);

        Task<bool> AddAsync(string email, string managerId);

        Task DeleteAsync(string id, string managerId);
    }
}
