namespace QuizHut.Web.ViewModels
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public string StatusCode { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
    }
}
