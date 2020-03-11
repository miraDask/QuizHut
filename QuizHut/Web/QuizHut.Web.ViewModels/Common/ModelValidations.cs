namespace QuizHut.Web.ViewModels.Shared
{
    public static class ModelValidations
    {
        public static class Quizzes
        {
            public const int NameMinLength = 3;

            public const int NameMaxLength = 1000;
        }

        public static class Password
        {
            public const int PasswordMinLength = 6;

            public const int PasswordMaxLength = 16;
        }

        public static class Groups
        {
            public const int NameMinLength = 2;

            public const int NameMaxLength = 50;
        }

        public static class Events
        {
            public const int NameMinLength = 2;

            public const int NameMaxLength = 50;
        }

        public static class Categories
        {
            public const int NameMinLength = 2;

            public const int NameMaxLength = 50;
        }

        public static class Answers
        {
            public const int TextMinLength = 1;

            public const int TextMaxLength = 1000;
        }

        public static class Question
        {
            public const int TextMinLength = 3;

            public const int TextMaxLength = 1000;
        }

        public static class Error
        {
            public const string RangeMessage = "The {0} must be at least {2} and at max {1} characters long.";

            public const string DateFormatMessage = @"Input format should be ""dd/MM/yyyy"".";

            public const string TimeActiveFromFormatMessage = @"Input format should be ""HH:mm"".";

            public const string TimeActiveToMessage = @"Input format should be ""HH:mm"" and with value up to 23:59.";
        }

        internal static class RegEx
        {
            public const string Date = @"^((0[1-9]|[12]\d|3[01])\/(0[1-9]|1[0-2])\/[12]\d{3})$";
            public const string TimeActiveFrom = @"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$";
            public const string TimeActiveTo = @"^(?!00)(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$";
        }
    }
}
