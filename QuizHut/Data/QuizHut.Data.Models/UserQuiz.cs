namespace QuizHut.Data.Models
{
    using System.Collections;
    public class UserQuiz
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public int QuizId { get; set; }

        public Quiz Quiz { get; set; }

        public int Points { get; set; }

        public bool QuizIsPending { get; set; }
    }
}
