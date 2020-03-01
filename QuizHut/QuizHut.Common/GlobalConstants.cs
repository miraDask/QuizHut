namespace QuizHut.Common
{
    public static class GlobalConstants
    {
        public const string SystemName = "QuizHut";

        public const string Administration = "Administration";

        public const string AdministratorRoleName = "Administrator";

        public const string ModeratorRoleName = "Moderator";

        public const string AdministratorAndModeratorAuthorizationString = "Administrator, Moderator";

        public const string RotativaPath = "wwwroot/Rotativa";

        public const string ExceptionHandlerPath = "/Home/Error";

        public const string StatusCodePath = "/Home/StatusCode?code={0}";

        public const int CookieTimeOut = 4;
    }
}
