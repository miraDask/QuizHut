namespace QuizHut.Services.Quiz
{
    
    using System.Threading.Tasks;
    using QuizHut.Services.Mapping;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using AutoMapper;
    using QuizHut.Web.ViewModels.Quiz;
    using System;
    using Microsoft.AspNetCore.Identity;

    public class QuizService : IQuizService
    {
        private readonly IDeletableEntityRepository<Quiz> repository;
        private readonly IMapper mapper;

        public QuizService(IDeletableEntityRepository<Quiz> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }


        public async Task AddNewQuizAsync(InputQuizViewModel quizModel)
        {
            var quiz = new Quiz
            {
                Name = quizModel.Name,
                Description = quizModel.Description,
                ActivationDateAndTime = Convert.ToDateTime(quizModel.ActivationDate),
                Duration = quizModel.Duration,
                CreatorId = quizModel.CreatorId,
            };

            await this.repository.AddAsync(quiz);
            await this.repository.SaveChangesAsync();
        }

        //public Task AddNewQuizAsync<TViewmodel>(TViewmodel viewmodel)
        //{
        //    var quiz = viewmodel.From<>
        //}
    }
}
