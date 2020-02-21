namespace QuizHut.Web.ViewModels.Groups
{
    using System.Collections.Generic;

    using QuizHut.Web.ViewModels.Participants;
    using QuizHut.Web.ViewModels.Quizzes;

    public class GroupDetailsViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IList<QuizAssignViewModel> Quizzes { get; set; } = new List<QuizAssignViewModel>();

        public IList<ParticipantViewModel> Participants { get; set; }
    }
}
