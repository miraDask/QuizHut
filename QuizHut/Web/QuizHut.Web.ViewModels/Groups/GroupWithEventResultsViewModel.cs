namespace QuizHut.Web.ViewModels.Groups
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Results;

    // no use for now - concider delete
    public class GroupWithEventResultsViewModel : IMapFrom<Group>
    {
        public string Name { get; set; }

        public IEnumerable<ResultViewModel> Results { get; set; }
    }
}
