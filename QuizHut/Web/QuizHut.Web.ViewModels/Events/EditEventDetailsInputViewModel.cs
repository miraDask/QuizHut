namespace QuizHut.Web.ViewModels.Events
{
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Shared;

    public class EditEventDetailsInputViewModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public string Id { get; set; }

        [Required]
        [StringLength(
         ModelValidations.Events.NameMaxLength,
         ErrorMessage = ModelValidations.Error.RangeMessage,
         MinimumLength = ModelValidations.Events.NameMinLength)]
        public string Name { get; set; }

        [Required]
        [RegularExpression(ModelValidations.RegEx.Date, ErrorMessage = ModelValidations.Error.DateFormatMessage)]
        public string ActivationDate { get; set; }

        [Required]
        [RegularExpression(ModelValidations.RegEx.TimeActiveFrom, ErrorMessage = ModelValidations.Error.TimeActiveFromFormatMessage)]
        public string ActiveFrom { get; set; }

        [Required]
        [RegularExpression(ModelValidations.RegEx.TimeActiveTo, ErrorMessage = ModelValidations.Error.TimeActiveToMessage)]
        public string ActiveTo { get; set; }

        public string Error { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, EditEventDetailsInputViewModel>()
             .ForMember(
                   x => x.ActivationDate,
                   opt => opt.MapFrom(x => x.ActivationDateAndTime.Date.ToString("dd/MM/yyyy")))
             .ForMember(
                   x => x.ActiveFrom,
                   opt => opt.MapFrom(x => $"{x.ActivationDateAndTime.Hour.ToString("D2")}:{x.ActivationDateAndTime.Minute.ToString("D2")}"))
             .ForMember(
                   x => x.ActiveTo,
                   opt => opt.MapFrom(x => $"{x.ActivationDateAndTime.Add(x.DurationOfActivity).Hour.ToString("D2")}:{x.ActivationDateAndTime.Add(x.DurationOfActivity).Minute.ToString("D2")}"));
        }
    }
}
