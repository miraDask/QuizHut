namespace QuizHut.Data.Models
{
    using QuizHut.Data.Common.Models;
    using System;

    public class Event : BaseDeletableModel<string>
    {
        public Event()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public DateTime? ActivationDateAndTime { get; set; }

        public TimeSpan? DurationOfActivity { get; set; }

        public string GroupId { get; set; }

        public virtual Group Group { get; set; }

        public string QuizId { get; set; }

        public virtual Quiz Quiz { get; set; }
    }
}
