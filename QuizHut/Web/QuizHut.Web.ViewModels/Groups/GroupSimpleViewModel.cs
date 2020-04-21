namespace QuizHut.Web.ViewModels.Groups
{
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class GroupSimpleViewModel : IMapFrom<Group>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
