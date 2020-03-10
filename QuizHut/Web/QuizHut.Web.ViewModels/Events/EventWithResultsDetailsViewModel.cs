namespace QuizHut.Web.ViewModels.Events
{
    using System.Collections.Generic;

    using AutoMapper;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Results;

    public class EventWithResultsDetailsViewModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public EventWithResultsDetailsViewModel()
        {
            this.Results = new HashSet<ResultViewModel>();
        }

        public string Name { get; set; }

        public string GroupName { get; set; }

        public int StudentsCount { get; set; }

        public IEnumerable<ResultViewModel> Results { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, EventWithResultsDetailsViewModel>()
                .ForMember(
                    x => x.StudentsCount,
                    opt => opt.MapFrom(x => x.Group.StudentstGroups.Count));
        }
    }
}
