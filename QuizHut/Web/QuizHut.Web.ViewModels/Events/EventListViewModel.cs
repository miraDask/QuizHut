namespace QuizHut.Web.ViewModels.Events
{
    using System;

    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class EventListViewModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime ActivationDateAndTime { get; set; }

        public TimeSpan DurationOfActivity { get; set; }

        public bool IsDeleted { get; set; }

        public string StartDate
            => $"{this.ActivationDateAndTime.ToLocalTime().Date.ToString("dd/MM/yyyy")}";

        public string Duration
            => $"{this.ActivationDateAndTime.ToLocalTime().Hour.ToString("D2")}:{this.ActivationDateAndTime.ToLocalTime().Minute.ToString("D2")}" +
               $" - {this.ActivationDateAndTime.ToLocalTime().Add(this.DurationOfActivity).Hour.ToString("D2")}:{this.ActivationDateAndTime.ToLocalTime().Add(this.DurationOfActivity).Minute.ToString("D2")}";

        public string Status { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, EventListViewModel>()
                .ForMember(
                    x => x.Status,
                    opt => opt.MapFrom(x => x.Status.ToString()));
        }
    }
}
