namespace QuizHut.Web.ViewModels.Quizzes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Questions;

    public class InputQuizViewModel : IMapFrom<Quiz>, IHaveCustomMappings
    {
        public string Name { get; set; }

        public string CreatorId { get; set; }

        public string Description { get; set; }

        public string ActivationDate { get; set; }

        [Required]
        public string Password { get; set; }

        public int? Duration { get; set; }

        public IList<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Quiz, InputQuizViewModel>()
                .ForMember(
                    x => x.ActivationDate,
                    opt => opt.MapFrom(x => x.ActivationDateAndTime != null ? x.ActivationDateAndTime.Value.ToString("dd/MM/yyyy") : string.Empty));
        }
    }
}
