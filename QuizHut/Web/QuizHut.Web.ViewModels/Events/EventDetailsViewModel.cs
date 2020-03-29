namespace QuizHut.Web.ViewModels.Events
{
    using System;
    using System.Collections.Generic;

    using AutoMapper;
    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Groups;

    public class EventDetailsViewModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public EventDetailsViewModel()
        {
            this.Groups = new HashSet<GroupAssignViewModel>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public Status Status { get; set; }

        public DateTime ActivationDateAndTime { get; set; }

        public TimeSpan DurationOfActivity { get; set; }

        public string ActivationDate
         => $"{this.ActivationDateAndTime.ToLocalTime().Date.ToString("dd/MM/yyyy")}";

        public string ActiveFrom
        => $"{this.ActivationDateAndTime.ToLocalTime().Hour.ToString("D2")}:{this.ActivationDateAndTime.ToLocalTime().Minute.ToString("D2")}";

        public string ActiveTo
        => $"{this.ActivationDateAndTime.ToLocalTime().Add(this.DurationOfActivity).Hour.ToString("D2")}:{this.ActivationDateAndTime.ToLocalTime().Add(this.DurationOfActivity).Minute.ToString("D2")}";

        public string QuizName { get; set; }

        public string QuizId { get; set; }

        public string ConfirmationMessage { get; set; }

        public IEnumerable<GroupAssignViewModel> Groups { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, EventDetailsViewModel>()
            .ForMember(
                  x => x.QuizId,
                  opt => opt.MapFrom(x => x.QuizId))
            .ForMember(
                  x => x.QuizName,
                  opt => opt.MapFrom(x => x.Quiz.Name));
        }
    }
}
