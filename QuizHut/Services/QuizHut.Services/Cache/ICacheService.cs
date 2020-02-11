namespace QuizHut.Services.Cache
{
    using System.Threading.Tasks;

    using QuizHut.Web.ViewModels.Quizzes;

    public interface ICacheService
    {
        Task SaveQuizModelToCacheAsync(InputQuizViewModel model);

        Task<InputQuizViewModel> GetQuizModelFromCacheAsync();

        Task DeleteQuestionAsync(string id);

        Task DeleteAnswerAsync(string id);
    }
}