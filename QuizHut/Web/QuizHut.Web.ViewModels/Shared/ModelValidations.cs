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
            internal const string Message = "The {0} must be at least {2} and at max {1} characters long.";
        }
    }
}
