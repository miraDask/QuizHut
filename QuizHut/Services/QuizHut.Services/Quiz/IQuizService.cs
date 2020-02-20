namespace QuizHut.Services.Quiz
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IQuizService
    {
        Task<string> AddNewQuizAsync(string name, string description, string activationDate, int? duration, string creatorId, string password);

        Task<IList<T>> GetAllAsync<T>();

        Task<T> GetQuizByIdAsync<T>(string id);

        Task<string> GetQuizIdByPasswordAsync(string password);

        Task DeleteByIdAsync(string id);

        Task UpdateAsync(string id, string name, string description, string activationDate, int? duration, string password);
    }
}
