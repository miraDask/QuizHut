namespace QuizHut.Web.ViewModels.Events
{
    using System.Collections.Generic;

    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Quizzes;

    public class EventDetailsViewModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public EventDetailsViewModel()
        {
            this.Groups = new HashSet<GroupAssignViewModel>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string ActivationDate { get; set; }

        public string ActiveFrom { get; set; }

        public string ActiveTo { get; set; }

        public string QuizName { get; set; }

        public string QuizId { get; set; }

        public IEnumerable<GroupAssignViewModel> Groups { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
              configuration.CreateMap<Event, EventDetailsViewModel>()
              .ForMember(
                    x => x.QuizId,
                    opt => opt.MapFrom(x => x.QuizId))
              .ForMember(
                    x => x.QuizName,
                    opt => opt.MapFrom(x => x.Quiz.Name))
                .ForMember(
                    x => x.ActivationDate,
                    opt => opt.MapFrom(x => x.ActivationDateAndTime.ToString("dd/MM/yyyy")))
               .ForMember(
                    x => x.ActiveFrom,
                    opt => opt.MapFrom(
                        x => $"{x.ActivationDateAndTime.Hour.ToString("D2")}:{x.ActivationDateAndTime.Minute.ToString("D2")}"))
               .ForMember(
                    x => x.ActiveTo,
                    opt => opt.MapFrom(
                        x => $"{x.ActivationDateAndTime.Add(x.DurationOfActivity).Hour.ToString("D2")}:{x.ActivationDateAndTime.Add(x.DurationOfActivity).Minute.ToString("D2")}"));
        }
    }
}
