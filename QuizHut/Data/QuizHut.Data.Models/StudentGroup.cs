namespace QuizHut.Data.Models
{
    public class StudentGroup
    {
        public string StudentId { get; set; }

        public virtual ApplicationUser Student { get; set; }

        public string GroupId { get; set; }

        public virtual Group Group { get; set; }
    }
}
