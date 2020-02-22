namespace QuizHut.Web.ViewModels.Groups
{
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class EditGroupNameInputViewModel : IMapFrom<Group>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
