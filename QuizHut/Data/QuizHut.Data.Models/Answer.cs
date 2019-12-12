namespace QuizHut.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Validations.DataValidation.Answer;

    public class Answer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(TextMaxLength, MinimumLength = TextMinLength)]
        public string Text { get; set; }

        public bool IsTheRightAnswer { get; set; }

        public int QuestionId { get; set; }

        public Question Question { get; set; }
    }
}
