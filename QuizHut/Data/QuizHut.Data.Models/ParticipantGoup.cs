namespace QuizHut.Data.Models
{
    // TODO what to inherit?
    public class ParticipantGoup
    {
        public string GroupId { get; set; }

        public virtual Group Group { get; set; }

        public string PrticipantId { get; set; }

        public virtual ApplicationUser Participant { get; set; }
    }
}
