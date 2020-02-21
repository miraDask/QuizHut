namespace QuizHut.Data.Models
{
    using System;

    using QuizHut.Data.Common.Models;

    public class ParticipantGroup : BaseDeletableModel<string>
    {
        public ParticipantGroup()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string ParticipantId { get; set; }

        public virtual ApplicationUser Participant { get; set; }

        public string GroupId { get; set; }

        public virtual Group Group { get; set; }
    }
}
