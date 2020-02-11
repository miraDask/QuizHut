namespace QuizHut.Services.Answer
{
    using System.Threading.Tasks;

    public interface IAnswerService
    {
        Task AddNewAnswerAsync(string answerText, bool isRightAnswer, string questionId);

        Task UpdateAsync(string id, string text, bool isRightAnswer);

        Task<string> GetAnswerId(string questionId, string answerText);

        Task Delete(string id);
    }
}
