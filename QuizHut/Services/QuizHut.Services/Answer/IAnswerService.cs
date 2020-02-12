namespace QuizHut.Services.Answer
{
    using QuizHut.Web.ViewModels.Answers;
    using System.Threading.Tasks;

    public interface IAnswerService
    {
        Task AddNewAnswerAsync(string answerText, bool isRightAnswer, string questionId);

        Task UpdateAsync(string id, string text, bool isRightAnswer);

        Task<AnswerViewModel> GetAnswerModelAsync(string id);

        Task Delete(string id);
    }
}
