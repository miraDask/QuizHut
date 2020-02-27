namespace QuizHut.Web.ViewModels.Participants
{
    using System.ComponentModel.DataAnnotations;

    public class ParticipantInputViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
