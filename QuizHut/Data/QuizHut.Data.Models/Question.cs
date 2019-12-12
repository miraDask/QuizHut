namespace QuizHut.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Validations.DataValidation.Question;

    public class Question
    {
        public int Id { get; set; }

        [Required]
        [StringLength(TextMaxLength, MinimumLength = TextMinLength)]
        public string Text { get; set; }

        public ICollection<Answer> Answers { get; set; } = new HashSet<Answer>();

        public int QuizId { get; set; }

        public Quiz Quiz { get; set; }
    }
}
