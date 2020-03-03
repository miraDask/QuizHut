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
            this.QuizzesGroups = new HashSet<QuizGroup>();
        }

        public string Name { get; set; }

        public string CreatorId { get; set; }

        public virtual ApplicationUser Creator { get; set; }

        public virtual ICollection<StudentGroup> StudentstGroups { get; set; }

        public virtual ICollection<QuizGroup> QuizzesGroups { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
