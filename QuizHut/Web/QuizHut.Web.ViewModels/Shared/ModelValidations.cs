namespace QuizHut.Web.ViewModels.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class ModelValidations
    {
        public static class Quizzes
        {
            public const int NameMinLength = 3;

            public const int NameMaxLength = 1000;

            public const int ActivationDateLength = 10;

        }
    }
}
