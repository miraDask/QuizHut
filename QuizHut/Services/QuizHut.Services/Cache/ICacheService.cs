namespace QuizHut.Services.Cache
{
    using QuizHut.Web.ViewModels.Quiz;

    public interface ICacheService
    {
        void SaveQuizModelToCache(InputQuizViewModel model);

        InputQuizViewModel GetQuizModelFromCache();
    }
}