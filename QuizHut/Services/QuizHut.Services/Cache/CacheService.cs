namespace QuizHut.Services.Cache
{
    using System.Linq;

    using Microsoft.Extensions.Caching.Memory;
    using QuizHut.Web.ViewModels.Question;
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

        public void DeleteQuestion(string id)
        {
            var model = this.GetQuizModelFromCache();
            var question = model.Questions.Where(x => x.Id == id).FirstOrDefault();
            question.IsDeleted = true;
            this.SaveQuizModelToCache(model);
        }

        public void DeleteAnswer(string id)
        {
            var model = this.GetQuizModelFromCache();
            var answers = model.Questions.SelectMany(x => x.Answers).ToList();
            var answer = answers.Where(x => x.Id == id).FirstOrDefault();
            answer.IsDeleted = true;
            this.SaveQuizModelToCache(model);
        }
    }
}
