namespace QuizHut.Common
{
    public static class GlobalConstants
    {
        public const string SystemName = "QuizHut";

        public const string Administration = "Administration";

        public const string AdministratorRoleName = "Administrator";

        public const string TeacherRoleName = "Teacher";

        public const string AdministratorAndTeacherAuthorizationString = "Administrator, Teacher";

        public const string RotativaPath = "wwwroot/Rotativa";

        public const string ExceptionHandlerPath = "/Home/Error";

        public const string StatusCodePath = "/Home/StatusCode?code={0}";

        public const int CookieTimeOut = 4;

        public static class DataSeeding
        {
            public const string Password = "123456";

            public const string AdminName = "Admin";

            public const string AdminEmail = "admin@admin.com";

            public const string TeacherName = "Teacher";

            public const string TeacherEmail = "teacher@teacher.com";

            public const string StudentName = "Student";

            public const string StudentEmail = "student@student.com";
        }

        public static class ErrorMessages
        {
            public const string EmptyPasswordField = "You should enter password if you want to use this shortcut! Try again!";

            public const string QuizNotFound = "There is no quiz with password {0}! Try again!";

            public const string PermissionDenied = "You do not have a permission to participate in this quiz!";
        }
    }
}
