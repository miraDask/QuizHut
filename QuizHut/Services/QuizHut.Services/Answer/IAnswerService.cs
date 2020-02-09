namespace QuizHut.Services.Answer
{
    using System.Threading.Tasks;

    public interface IAnswerService
    {
        Task AddNewAnswerAsync(string answerText, bool isRightAnswer, string questionId);
    }
}
