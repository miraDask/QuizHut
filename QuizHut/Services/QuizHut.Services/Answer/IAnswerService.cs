namespace QuizHut.Services.Answer
{
    using System.Threading.Tasks;

    using QuizHut.Web.ViewModels.Answer;

    public interface IAnswerService
    {
        Task AddNewAnswerAsync(AnswerViewModel answerViewModel);
    }
}
