namespace QuizHut.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Web.Controllers;

    [Authorize(Roles = "Administrator, Moderator")]
    [Area("Administration")]
    public class AdministrationController : BaseController
    {
    }
}
