namespace QuizHut.Services.Questions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IQuestionsService
    {
        Task<string> CreateQuestionAsync(string quizId, string questionText);

        Task DeleteQuestionByIdAsync(string id);

        Task UpdateAllQuestionsInQuizNumeration(string quizId);

        Task Update(string id, string text);

        Task<T> GetByIdAsync<T>(string id);

        Task<T> GetQuestionByQuizIdAndNumberAsync<T>(string quizId, int number);

        Task<IList<T>> GetAllByQuizIdAsync<T>(string id);

        Task<int> GetAllByQuizIdCountAsync(string id);
    }
}
