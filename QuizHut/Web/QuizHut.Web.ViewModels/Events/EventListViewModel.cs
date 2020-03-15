namespace QuizHut.Web.ViewModels.Events
{
    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class EventListViewModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public string StartDate { get; set; }

        public string Duration { get; set; }

        public string Status { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, EventListViewModel>()
                .ForMember(
                    x => x.StartDate,
                    opt => opt.MapFrom(
                        x => $"{x.ActivationDateAndTime.Date.ToString("dd/MM/yyyy")}"))
                .ForMember(
                    x => x.Duration,
                    opt => opt.MapFrom(
                        x => $"{x.ActivationDateAndTime.Hour.ToString("D2")}:{x.ActivationDateAndTime.Minute.ToString("D2")}" +
                        $" - {x.ActivationDateAndTime.Add(x.DurationOfActivity).Hour.ToString("D2")}:{x.ActivationDateAndTime.Add(x.DurationOfActivity).Minute.ToString("D2")}"))
                .ForMember(
                    x => x.Status,
                    opt => opt.MapFrom(x => x.Status.ToString()));
        }
    }
}
