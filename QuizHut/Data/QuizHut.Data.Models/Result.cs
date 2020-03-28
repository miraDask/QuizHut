namespace QuizHut.Data.Models
{
    using System;

    using QuizHut.Data.Common.Models;

    public class Result : BaseDeletableModel<string>
    {
        public Result()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public int Points { get; set; }

        public int MaxPoints { get; set; }

        public string StudentId { get; set; }

        public virtual ApplicationUser Student { get; set; }

        public string EventId { get; set; }

        public virtual Event Event { get; set; }

        public string EventName { get; set; }

        public string QuizName { get; set; }

        public DateTime EventActivationDateAndTime { get; set; }
    }
}
