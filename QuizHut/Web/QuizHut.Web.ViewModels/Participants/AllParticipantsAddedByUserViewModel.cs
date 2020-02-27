namespace QuizHut.Web.ViewModels.Participants
{
    using System.Collections.Generic;

    public class AllParticipantsAddedByUserViewModel
    {
        public string UserId { get; set; }

        public ParticipantInputViewModel NewParticipant { get; set; }

        public IList<ParticipantViewModel> Participants { get; set; } = new List<ParticipantViewModel>();
    }
}
