namespace QuizHut.Data.Models.Validations
{
   
    internal static class DataValidation
    {
        internal static class Answer
        {
            internal const int TextMinLength = 3;
            internal const int TextMaxLength = 100;
        }

        internal static class Question
        {
            internal const int TextMinLength = 3;
            internal const int TextMaxLength = 100;
        }

        internal static class Category
        {
            internal const int NameMinLength = 2;
            internal const int NameMaxLength = 30;
        }

        internal static class Quiz
        {
            internal const int NameMinLength = 3;
            internal const int NameMaxLength = 100;
        }

        internal static class Code
        {
            internal const int TextMinLength = 3;
            internal const int TextMaxLength = 10;
        }

        internal static class User
        {
            internal const int UsernameMinLength = 6;
            internal const int UsernameMaxLength = 10;

            internal const int NameMinLength = 3;
            internal const int NameMaxLength = 30;
            
            internal const int PasswordMinLength = 6;
            internal const int PasswordMaxLength = 10;
            internal const string PasswordFormat = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,10}$";
        }
    }
}
