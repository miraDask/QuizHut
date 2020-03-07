namespace QuizHut.Web.ViewModels.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Quizzes;

    public class EventDetailsViewModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ActivationDate { get; set; }

        public string ActiveFrom { get; set; }

        public string ActiveTo { get; set; }

        public QuizAssignViewModel Quiz { get; set; }

        public IEnumerable<GroupAssignViewModel> Groups { get; set; } = new HashSet<GroupAssignViewModel>();

        public void CreateMappings(IProfileExpression configuration)
        {
              configuration.CreateMap<Event, EventDetailsViewModel>()
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
                        : string.Empty))
                 .ForMember(
                    x => x.Groups,
                    opt => opt.MapFrom(x => x.EventsGroups.Select(x => new GroupAssignViewModel() { Id = x.GroupId, IsAssigned = true, Name = x.Group.Name })))
                   .ForMember(
                    x => x.Quiz,
                    opt => opt.MapFrom(x => new QuizAssignViewModel() { Name = x.Quiz.Name, IsAssigned = true, Id = x.Quiz.Id }));
        }
    }
}
