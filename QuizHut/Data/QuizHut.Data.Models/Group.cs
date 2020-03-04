namespace QuizHut.Data.Models
{
    using System;
    using System.Collections.Generic;

    using QuizHut.Data.Common.Models;

    public class Group : BaseDeletableModel<string>
    {
        public Group()
        {
            this.Id = Guid.NewGuid().ToString();
            this.StudentstGroups = new HashSet<StudentGroup>();
            this.EventsGroups = new HashSet<EventGroup>();
        }

        public string Name { get; set; }

        public string CreatorId { get; set; }

        public virtual ApplicationUser Creator { get; set; }

        public virtual ICollection<StudentGroup> StudentstGroups { get; set; }

        public virtual ICollection<EventGroup> EventsGroups { get; set; }
    }
}
