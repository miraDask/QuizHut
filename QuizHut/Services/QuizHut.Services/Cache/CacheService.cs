namespace QuizHut.Services.Cache
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Caching.Distributed;
    using Newtonsoft.Json;
    using QuizHut.Services.Common;
    using QuizHut.Web.ViewModels.Quiz;

    public class CacheService : ICacheService
    {
        private readonly IDistributedCache cache;

        public CacheService(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task<InputQuizViewModel> GetQuizModelFromCacheAsync()
        {
            var json = await this.cache.GetStringAsync(ServicesConstants.RedisModelKey);
            var model = JsonConvert.DeserializeObject<InputQuizViewModel>(json);
            return model;
        }

        public async Task SaveQuizModelToCacheAsync(InputQuizViewModel model)
        {
            var modelJson = JsonConvert.SerializeObject(model);
            await this.cache.SetStringAsync(ServicesConstants.RedisModelKey, modelJson);
        }

        public async Task DeleteQuestionAsync(string id)
        {
            var model = await this.GetQuizModelFromCacheAsync();
            var question = model.Questions.Where(x => x.Id == id).FirstOrDefault();
            question.IsDeleted = true;
            await this.SaveQuizModelToCacheAsync(model);
        }

        public async Task DeleteAnswerAsync(string id)
        {
            var model = await this.GetQuizModelFromCacheAsync();
            var answers = model.Questions.SelectMany(x => x.Answers).ToList();
            var answer = answers.Where(x => x.Id == id).FirstOrDefault();
            answer.IsDeleted = true;
            await this.SaveQuizModelToCacheAsync(model);
        }
    }
}
