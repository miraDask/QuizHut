namespace QuizHut.Web.ViewModels.Events
{
    using System.Collections.Generic;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Results;

    public class EventWithResultsDetailsViewModel : IMapFrom<Event>
    {
        public EventWithResultsDetailsViewModel()
        {
            this.Results = new HashSet<ResultViewModel>();
        }

        public string Name { get; set; }

        public IEnumerable<ResultViewModel> Results { get; set; }
    }
}
