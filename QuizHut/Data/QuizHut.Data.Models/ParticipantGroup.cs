namespace QuizHut.Data.Models
{
    // TODO what to inherit?
    public class ParticipantGroup
    {
        public string ParticipantId { get; set; }

        public virtual ApplicationUser Participant { get; set; }

        public string GroupId { get; set; }

        public virtual Group Group { get; set; }
    }
}
