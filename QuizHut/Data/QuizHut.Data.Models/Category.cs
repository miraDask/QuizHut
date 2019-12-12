namespace QuizHut.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Validations.DataValidation.Category;

    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; set; }

        public ICollection<Quiz> Quizzes { get; set; }
    }
}
