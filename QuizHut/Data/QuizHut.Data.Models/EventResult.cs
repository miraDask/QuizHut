namespace QuizHut.Data.Models
{

    using QuizHut.Data.Common.Models;

    public class EventResult : BaseDeletableModel<string>
    {
        public string ResultId { get; set; }

        public Result Result { get; set; }

        public string EventId { get; set; }

        public virtual Event Event { get; set; }
    }
}
