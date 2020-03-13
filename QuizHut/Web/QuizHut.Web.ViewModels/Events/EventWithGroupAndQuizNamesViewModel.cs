namespace QuizHut.Web.ViewModels.Events
{
    using System.Collections.Generic;
    using System.Linq;
    
    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Results;

    public class EventWithGroupAndQuizNamesViewModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public EventWithGroupAndQuizNamesViewModel()
        {
            this.GroupsNames = new HashSet<string>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string QuizName { get; set; }

        public IEnumerable<string> GroupsNames { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, EventWithGroupAndQuizNamesViewModel>()
                .ForMember(
                   x => x.GroupsNames,
                   opt => opt.MapFrom(x => x.EventsGroups.Select(x => x.Group.Name)));
        }
    }
}
