namespace QuizHut.Data.Models
{
    public class EventGroup
    {
        public string GroupId { get; set; }

        public virtual Group Group { get; set; }

        public string EventId { get; set; }

        public virtual Event Event { get; set; }
    }
}
