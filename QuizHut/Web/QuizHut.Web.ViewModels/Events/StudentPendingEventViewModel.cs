namespace QuizHut.Web.ViewModels.Events
{
    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class StudentPendingEventViewModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public string Name { get; set; }

        public string QuizName { get; set; }

        public string Date { get; set; }

        public string Duration { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, StudentPendingEventViewModel>()
                .ForMember(
                    x => x.Date,
                    opt => opt.MapFrom(
                        x => x.ActivationDateAndTime.Date.ToString("dd/MM/yyyy")))
                .ForMember(
                    x => x.Duration,
                    opt => opt.MapFrom(
                        x => $"{x.ActivationDateAndTime.ToLocalTime().Hour.ToString("D2")}:{x.ActivationDateAndTime.ToLocalTime().Minute.ToString("D2")}" +
                        $" - {x.ActivationDateAndTime.ToLocalTime().Add(x.DurationOfActivity).Hour.ToString("D2")}:{x.ActivationDateAndTime.ToLocalTime().Add(x.DurationOfActivity).Minute.ToString("D2")}"));
        }
    }
}
