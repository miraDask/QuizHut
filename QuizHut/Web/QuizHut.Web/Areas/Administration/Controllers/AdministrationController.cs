namespace QuizHut.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Web.Controllers;

    [Authorize(Roles = GlobalConstants.AdministratorAndTeacherAuthorizationString)]
    [Area(GlobalConstants.Administration)]
    public class AdministrationController : BaseController
    {
    }
}
