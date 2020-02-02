namespace QuizHut.Services.Cache
{
    using Microsoft.Extensions.Caching.Memory;
    using QuizHut.Web.ViewModels.Quiz;

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache cache;

        public CacheService(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public InputQuizViewModel GetQuizModelFromCache()
        {
            return this.cache.Get<InputQuizViewModel>("quiz");
        }

        public void SaveQuizModelToCache(InputQuizViewModel model)
        {
            this.cache.Set("quiz", model);
        }
    }
}
