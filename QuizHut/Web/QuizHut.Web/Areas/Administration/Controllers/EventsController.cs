namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    public class EventsController : AdministrationController
    {
        public IActionResult AllEventsCreatedByTeacher()
        {
            return this.View();
        }
    }
}