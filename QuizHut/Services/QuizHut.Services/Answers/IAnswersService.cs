namespace QuizHut.Services.Answers
{
    using System.Threading.Tasks;

    using QuizHut.Web.ViewModels.Answers;

    public interface IAnswersService
    {
        Task AddNewAnswerAsync(string answerText, bool isRightAnswer, string questionId);

        Task UpdateAsync(string id, string text, bool isRightAnswer);

        Task<AnswerViewModel> GetAnswerModelAsync(string id);

        Task Delete(string id);
    }
}
