namespace QuizHut.Services.Quiz
{
    
    using System.Threading.Tasks;
    using QuizHut.Services.Mapping;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;

    public class QuizService : IQuizService
    {
        private readonly IDeletableEntityRepository<Quiz> repository;

        public QuizService(IDeletableEntityRepository<Quiz> repository)
        {
            this.repository = repository;
        }

        public Task AddNewQuizAsync<TViewmodel>(TViewmodel viewmodel)
        {
            throw new System.NotImplementedException();
        }

        //public Task AddNewQuizAsync<TViewmodel>(TViewmodel viewmodel)
        //{
        //    var quiz = viewmodel.From<>
        //}
    }
}
