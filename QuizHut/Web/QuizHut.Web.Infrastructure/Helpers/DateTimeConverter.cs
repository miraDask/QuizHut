namespace QuizHut.Web.Infrastructure.Helpers
{
    using System;

    using TimeZoneConverter;

    public class DateTimeConverter : IDateTimeConverter
    {
        public string GetDate(DateTime activationDateAndTime, string timeZoneIana)
        {
            var activationDateAndTimeToUserLocalTime = this.GetDateTimeLocalToTheUser(activationDateAndTime, timeZoneIana);
            return activationDateAndTimeToUserLocalTime.Date.ToString("dd/MM/yyyy");
        }

        public string GetDurationString(DateTime activationDateAndTime, TimeSpan duration, string timeZoneIana)
        {
            var activationDateAndTimeToUserLocalTime = this.GetDateTimeLocalToTheUser(activationDateAndTime, timeZoneIana);

            return $"{activationDateAndTimeToUserLocalTime.Hour.ToString("D2")}" +
                   $":{activationDateAndTimeToUserLocalTime.Minute.ToString("D2")}" +
                   $" - {activationDateAndTimeToUserLocalTime.Add(duration).Hour.ToString("D2")}" +
                   $":{activationDateAndTimeToUserLocalTime.Add(duration).Minute.ToString("D2")}";
        }

        private DateTime GetDateTimeLocalToTheUser(DateTime activationDateAndTime, string timeZoneIana)
        {
            var windowsTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timeZoneIana));
            return TimeZoneInfo.ConvertTimeFromUtc(activationDateAndTime, windowsTimeZone);
        }
    }
}
