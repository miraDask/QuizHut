namespace QuizHut.Web.ViewModels.Shared
{

    internal static class ModelValidations
    {
        internal static class Quizzes
        {
            internal const int NameMinLength = 3;

            internal const int NameMaxLength = 1000;

            internal const int PasswordMinLength = 6;

            internal const int PasswordMaxLength = 10;
        }

        internal static class Groups
        {
            internal const int NameMinLength = 2;

            internal const int NameMaxLength = 50;
        }

        internal static class Categories
        {
            internal const int NameMinLength = 2;

            internal const int NameMaxLength = 50;
        }

        internal static class Answers
        {
            internal const int TextMinLength = 1;

            internal const int TextMaxLength = 1000;
        }

        internal static class Error
        {
            internal const string RangeMessage = "The {0} must be at least {2} and at max {1} characters long.";

            internal const string DateFormatMessage = @"Input format should be ""dd/MM/yyyy"".";

            internal const string TimeFormatMessage = @"Input format should be ""HH:mm"".";
        }

        internal static class RegEx
        {
            internal const string Date = @"^((0[1-9]|[12]\d|3[01])\/(0[1-9]|1[0-2])\/[12]\d{3})$";

            internal const string Time = @"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$";
        }
    }
}
