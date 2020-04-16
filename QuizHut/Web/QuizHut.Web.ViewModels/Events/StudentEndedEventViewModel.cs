namespace QuizHut.Web.ViewModels.Events
{
    using System;

    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Results;

    public class StudentEndedEventViewModel : IMapFrom<Event>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string QuizName { get; set; }

        public DateTime ActivationDateAndTime { get; set; }

        public string Date { get; set; }

        public ScoreViewModel Score { get; set; }
    }
}
