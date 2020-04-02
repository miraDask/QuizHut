namespace QuizHut.Web.ViewModels.Quizzes
{
    using System.Collections.Generic;

    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Questions;

    public class QuizDetailsViewModel : IMapFrom<Quiz>, IHaveCustomMappings
    {
        public QuizDetailsViewModel()
        {
            this.Questions = new List<QuestionViewModel>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string EventName { get; set; }

        public string Description { get; set; }

        public string Password { get; set; }

        public int? Timer { get; set; }

        public IList<QuestionViewModel> Questions { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Quiz, QuizDetailsViewModel>()
             .ForMember(
                   x => x.Password,
                   opt => opt.MapFrom(x => x.Password.Content));
        }
    }
}
