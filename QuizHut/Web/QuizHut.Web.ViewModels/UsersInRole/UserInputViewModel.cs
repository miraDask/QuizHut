namespace QuizHut.Web.ViewModels.UsersInRole
{
    using System.ComponentModel.DataAnnotations;

    public class UserInputViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool IsNotAdded { get; set; }

        public string RoleName { get; set; }
    }
}
