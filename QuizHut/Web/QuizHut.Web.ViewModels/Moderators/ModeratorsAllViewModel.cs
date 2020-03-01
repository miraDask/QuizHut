namespace QuizHut.Web.ViewModels.Moderators
{
    using System.Collections.Generic;

    using QuizHut.Web.ViewModels.Participants;

    public class ModeratorsAllViewModel
    {
        public ParticipantInputViewModel NewModerator { get; set; }

        public IList<ModeratorViewModel> Moderators { get; set; } = new List<ModeratorViewModel>();
    }
}
