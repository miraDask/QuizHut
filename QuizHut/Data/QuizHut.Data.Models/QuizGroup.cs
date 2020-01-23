namespace QuizHut.Data.Models
{
    // TODO what to inherit?
    public class QuizGroup
    {
        public string GroupId { get; set; }

        public virtual Group Group { get; set; }

        public int QuizId { get; set; }

        public virtual Quiz Quiz { get; set; }
    }
}
