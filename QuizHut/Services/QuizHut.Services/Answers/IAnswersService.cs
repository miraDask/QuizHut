namespace QuizHut.Services.Answers
{
    using System.Threading.Tasks;

    public interface IAnswersService
    {
        Task CreateAnswerAsync(string answerText, bool isRightAnswer, string questionId);

        Task UpdateAsync(string id, string text, bool isRightAnswer);

        Task<T> GetByIdAsync<T>(string id);

        Task DeleteAsync(string id);
    }
}
