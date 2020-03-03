namespace QuizHut.Services.Quizzes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IQuizzesService
    {
        Task<string> AddNewQuizAsync(string name, string description, int? timer, string creatorId, string password);

        Task<IList<T>> GetAllAsync<T>();

        Task<IList<T>> GetAllByCreatorIdAsync<T>(string id);

        Task<T> GetQuizByIdAsync<T>(string id);

        Task<string> GetQuizIdByPasswordAsync(string password);

        Task DeleteByIdAsync(string id);

        Task UpdateAsync(string id, string name, string description, int? timer, string password);

        Task<string> GetIdByPassword(string password);
    }
}
