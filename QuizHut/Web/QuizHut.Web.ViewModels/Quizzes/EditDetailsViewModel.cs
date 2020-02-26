namespace QuizHut.Web.ViewModels.Quizzes
{
    using System;

    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class EditDetailsViewModel : IMapFrom<Quiz>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ActivationDate { get; set; }

        public string Password { get; set; }

        public string ActiveFrom { get; set; }

        public string ActiveTo { get; set; }

        public int? Timer { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Quiz, InputQuizViewModel>()
                .ForMember(
                    x => x.ActivationDate,
                    opt => opt.MapFrom(x => x.ActivationDateAndTime != null
                    ? x.ActivationDateAndTime.Value.ToString("dd/MM/yyyy") : string.Empty))
               .ForMember(
                    x => x.ActiveFrom,
                    opt => opt.MapFrom(x => x.ActivationDateAndTime != null
                    ? $"{x.ActivationDateAndTime.Value.Hour}:{x.ActivationDateAndTime.Value.Minute}" : string.Empty))
               .ForMember(
                    x => x.ActiveTo,
                    opt => opt.MapFrom(x => x.ActivationDateAndTime != null
                    ? $"{x.ActivationDateAndTime.Value.Add((TimeSpan)x.DurationOfActivity).Hour}:{x.ActivationDateAndTime.Value.Add((TimeSpan)x.DurationOfActivity).Minute}" : string.Empty));
        }
    }
}
