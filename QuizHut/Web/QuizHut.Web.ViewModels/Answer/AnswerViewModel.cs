namespace QuizHut.Web.ViewModels.Answer
{
    using System;

    public class AnswerViewModel
    {
        public AnswerViewModel()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        public string Text { get; set; }

        public bool IsRightAnswer { get; set; }

        public bool IsDeleted { get; set; }

        public int Count { get; set; }
    }
}
