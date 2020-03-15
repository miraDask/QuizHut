namespace QuizHut.Data.Models
{
    using System;
    using System.Collections.Generic;
    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Common.Models;

    public class Event : BaseDeletableModel<string>
    {
        public Event()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Results = new HashSet<Result>();
            this.EventsGroups = new HashSet<EventGroup>();
        }

        public string Name { get; set; }

        public string CreatorId { get; set; }

        public Status Status { get; set; }

        public virtual ApplicationUser Creator { get; set; }

        public DateTime ActivationDateAndTime { get; set; }

        public TimeSpan DurationOfActivity { get; set; }

        public string QuizId { get; set; }

        public virtual Quiz Quiz { get; set; }

        public ICollection<Result> Results { get; set; }

        public ICollection<EventGroup> EventsGroups { get; set; }
    }
}
