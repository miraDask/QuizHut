namespace QuizHut.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Validations.DataValidation.User;

    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(UsernameMaxLength, MinimumLength = UsernameMinLength)]
        public string Username { get; set; }

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(PasswordMaxLength, MinimumLength = PasswordMinLength)]
        [RegularExpression(PasswordFormat)]
        public string Password { get; set; }

        public ICollection<UserQuiz> UsersQuizes { get; set; } = new HashSet<UserQuiz>();

        public ICollection<Quiz> CreatedQuizzes { get; set; } = new HashSet<Quiz>();

    }
}
