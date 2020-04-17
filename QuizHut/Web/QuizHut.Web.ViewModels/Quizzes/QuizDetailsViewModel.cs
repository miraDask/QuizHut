namespace QuizHut.Web.ViewModels.Quizzes
{
    using System.Collections.Generic;

    using AutoMapper;
    using Ganss.XSS;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Questions;

    public class QuizDetailsViewModel : IMapFrom<Quiz>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string EventName { get; set; }

        public string Description { get; set; }

        public string SanitizedDescription => new HtmlSanitizer().Sanitize(this.Description);

        public string Password { get; set; }

        public int? Timer { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Quiz, QuizDetailsViewModel>()
             .ForMember(
                   x => x.Password,
                   opt => opt.MapFrom(x => x.Password.Content));
        }
    }
}
