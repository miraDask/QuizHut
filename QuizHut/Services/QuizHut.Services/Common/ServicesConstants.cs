namespace QuizHut.Services.Common
{
    public static class ServicesConstants
    {
        public const string AnswerNumberPattern = @"[\d]";
        public const int NoPointsValue = 0;
        public const int PointsValue = 1;
        public const int ValueCollectionMinCount = 1;
        public const string InvalidActivationDate = "Тhe activation date should not be earlier than today!";
        public const string InvalidStartingTime = "Invalid time for starting the event. Time for starting should not be earlier than current time!";
        public const string InvalidDurationOfActivity =
            "Invalid time for edning event. Time for ending should not be earlier than the time when event starts or the same!";

        public const string EventInvitationSubject = "Event invitation";
        public const string SenderEmail = "quizhutproject@gmail.com";
        public const string SenderName = "QuizHut team";
        public const string StringToReplace = "{password}";
        public const string Name = "Name";
    }
}
