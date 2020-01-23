namespace QuizHut.Data.Models
{
    using QuizHut.Data.Common.Models;

    // TODO what to inherit?
    public class QuizResult
    {
        public string ParticipantId { get; set; }

        public virtual ApplicationUser Participant { get; set; }

        public int QuizId { get; set; }

        public virtual Quiz Quiz { get; set; }

        public int Points { get; set; }
    }
}
