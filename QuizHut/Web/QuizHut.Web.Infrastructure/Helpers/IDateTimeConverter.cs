namespace QuizHut.Web.Infrastructure.Helpers
{
    using System;

    public interface IDateTimeConverter
    {
        public string GetDurationString(DateTime activationDateAndTime, TimeSpan duration, string timeZoneIana);

        public string GetDate(DateTime dateAndTime, string timeZoneIana);
    }
}
