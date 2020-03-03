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
    }
}
