namespace QuizHut.Web.ViewModels.Quizzes
{
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class QuizAssignViewModel : IMapFrom<Quiz>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string CreatorId { get; set; }

        public bool IsAssigned { get; set; }
    }
}
