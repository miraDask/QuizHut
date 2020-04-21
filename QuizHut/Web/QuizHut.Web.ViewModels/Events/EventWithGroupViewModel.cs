namespace QuizHut.Web.ViewModels.Events
{
    using System.Collections.Generic;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Groups;

    public class EventWithGroupViewModel : IMapFrom<Event>
    {
        public EventWithGroupViewModel()
        {
            this.Groups = new HashSet<GroupSimpleViewModel>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<GroupSimpleViewModel> Groups { get; set; }

        public GroupWithEventResultsViewModel Group { get; set; }

        public int PagesCount { get; set; }

        public int CurrentPage { get; set; }

        public int NextPage
        {
            get
            {
                if (this.CurrentPage >= this.PagesCount)
                {
                    return 1;
                }

                return this.CurrentPage + 1;
            }
        }

        public int PreviousPage
        {
            get
            {
                if (this.CurrentPage <= 1)
                {
                    return this.PagesCount;
                }

                return this.CurrentPage - 1;
            }
        }
    }
}
