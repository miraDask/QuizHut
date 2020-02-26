namespace QuizHut.Web.ViewModels.Quizzes
{
    using System;
    using System.ComponentModel.DataAnnotations;
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

        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Input format should be 'HH:MM'")]
        public string ActiveFrom { get; set; }

        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Input format should be 'HH:MM'")]
        public string ActiveTo { get; set; }

        public int? Timer { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Quiz, EditDetailsViewModel>()
                .ForMember(
                    x => x.ActivationDate,
                    opt => opt.MapFrom(x => x.ActivationDateAndTime != null
                    ? x.ActivationDateAndTime.Value.ToString("dd/MM/yyyy") : string.Empty))
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
