namespace QuizHut.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Validations.DataValidation.Quiz;

    public class Quiz
    {
        public int Id { get; set; }

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; set; }

        public TimeSpan? Duration { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? ActivationTime { get; set; }

        public bool IsActive { get; set; }

        public bool IsStarted { get; set; }

        public int? CategoryId { get; set; }

        public Category Category { get; set; }

        public int CodeId { get; set; }

        public Code Code { get; set; }

        public ICollection<Question> Questions { get; set; } = new HashSet<Question>();
    }
}
