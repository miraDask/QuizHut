namespace QuizHut.Data.Validations
{
    internal static class DataValidation
    {
        internal static class Answer
        {
            internal const int TextMaxLength = 1000;
        }

        internal static class Question
        {
            internal const int TextMaxLength = 1000;
        }

        internal static class Category
        {
            internal const int NameMaxLength = 100;
        }

        internal static class QuizGroup
        {
            internal const int NameMaxLength = 100;
        }

        internal static class Quiz
        {
            internal const int NameMaxLength = 1000;
        }

        internal static class QuizPassword
        {
            internal const int TextMaxLength = 10;
        }
    }
}
