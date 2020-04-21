namespace QuizHut.Web.ViewModels.Events
{
    using System.Collections.Generic;

    public class StudentEventsViewModel<T>
    {
        public IList<T> Events { get; set; } = new List<T>();

        public int PagesCount { get; set; }

        public int CurrentPage { get; set; }

        public string SearchType { get; set; }

        public string SearchString { get; set; }

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
