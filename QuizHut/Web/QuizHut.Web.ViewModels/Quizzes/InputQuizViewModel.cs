namespace QuizHut.Web.ViewModels.Quizzes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Questions;
    using QuizHut.Web.ViewModels.Shared;

    public class InputQuizViewModel : IMapFrom<Quiz>, IHaveCustomMappings
    {
        public string Id { get; set; }

        [Required]
        [StringLength(ModelValidations.Quizzes.NameMaxLength, MinimumLength = ModelValidations.Quizzes.NameMinLength)]
        public string Name { get; set; }

        public string CreatorId { get; set; }

        public string Description { get; set; }

        [StringLength(ModelValidations.Quizzes.ActivationDateLength, MinimumLength = ModelValidations.Quizzes.ActivationDateLength)]
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
