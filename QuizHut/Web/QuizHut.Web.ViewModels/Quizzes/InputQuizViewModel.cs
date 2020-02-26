namespace QuizHut.Web.ViewModels.Quizzes
{
    using System;
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

        public string ActiveFrom { get; set; }

        public string ActiveTo { get; set; }

        public int? Timer { get; set; }

        public IList<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Quiz, InputQuizViewModel>()
                .ForMember(
                    x => x.ActivationDate,
                    opt => opt.MapFrom(x => x.ActivationDateAndTime != null ? x.ActivationDateAndTime.Value.ToString("dd/MM/yyyy") : string.Empty))
               .ForMember(
                    x => x.ActiveFrom,
                    opt => opt.MapFrom(
                    x => x.DurationOfActivity != null
                    ? $"{x.ActivationDateAndTime.Value.Hour.ToString("D2")}:{x.ActivationDateAndTime.Value.Minute.ToString("D2")}" 
                    : string.Empty))
               .ForMember(
                    x => x.ActiveTo,
                    opt => opt.MapFrom(
                    x => x.DurationOfActivity != null
                    ? $"{x.ActivationDateAndTime.Value.Add((TimeSpan)x.DurationOfActivity).Hour.ToString("D2")}:{x.ActivationDateAndTime.Value.Add((TimeSpan)x.DurationOfActivity).Minute.ToString("D2")}"
                    : string.Empty));
        }
    }
}
