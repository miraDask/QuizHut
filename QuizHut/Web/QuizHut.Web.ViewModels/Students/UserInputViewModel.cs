namespace QuizHut.Web.ViewModels.Students
{
    using System.ComponentModel.DataAnnotations;

    public class UserInputViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool IsNotAdded { get; set; }
    }
}
