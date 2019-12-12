namespace QuizHut.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Validations.DataValidation.Code;
    
    public class Code
    {
        public int Id { get; set; }

        [Required]
        [StringLength(TextMaxLength, MinimumLength = TextMinLength)]
        public string Text { get; set; }

        public int QuizId { get; set; }

        public Quiz Quiz { get; set; }
    }
}
