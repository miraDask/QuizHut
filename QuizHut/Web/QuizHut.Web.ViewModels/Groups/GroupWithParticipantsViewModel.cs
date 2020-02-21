namespace QuizHut.Web.ViewModels.Groups
{
    using System.Collections.Generic;

    using QuizHut.Web.ViewModels.Participants;

    public class GroupWithParticipantsViewModel
    {
        public string GroupId { get; set; }

        public IList<ParticipantViewModel> Participants { get; set; }
    }
}
