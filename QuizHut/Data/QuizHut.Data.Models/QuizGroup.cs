namespace QuizHut.Data.Models
{
    public class QuizGroup
    {
        public string QuizId { get; set; }

        public virtual Quiz Quiz { get; set; }

        public string GroupId { get; set; }

        public virtual Group Group { get; set; }
    }
}
